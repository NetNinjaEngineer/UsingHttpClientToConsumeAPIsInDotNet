using System.Data.Common;
using System.Net;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace Movies.Client.Services
{
    public class DealingWithErrorsAndFaultsService : IIntegrationService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public DealingWithErrorsAndFaultsService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public async Task Run()
        {
            // await GetMovieAndDealWithInvalidRespones(_cancellationTokenSource.Token);
            await CreateMovieAndHandleValidationErrors(_cancellationTokenSource.Token);
        }

        private async Task CreateMovieAndHandleValidationErrors(CancellationToken cancellationToken)
        {
            var movieForCreation = new MovieForCreation { Title = "Pulpe Fiction" };
            var serializedMovieForCreation = JsonConvert.SerializeObject(movieForCreation);

            var httpClient = _httpClientFactory.CreateClient("MoviesClient");
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, "api/Movies");
            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            requestMessage.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            requestMessage.Content = new StringContent(serializedMovieForCreation);

            requestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            using (var responseMessage = await httpClient.SendAsync(requestMessage,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken))
            {
                var streamContent = await responseMessage.Content.ReadAsStreamAsync(CancellationToken.None);
                if (!responseMessage.IsSuccessStatusCode)
                {
                    if (responseMessage.StatusCode == HttpStatusCode.UnprocessableEntity)
                    {
                        var validationErrors = await responseMessage.Content.ReadAsStringAsync(CancellationToken.None);
                        Console.WriteLine(validationErrors);
                        return;
                    }

                    else if (responseMessage.StatusCode == HttpStatusCode.Unauthorized)
                        return;
                }

                responseMessage.EnsureSuccessStatusCode();
                var createdMovie = streamContent.ReadAndDeserializeFromJson<Movie>();

            }
        }


        private async Task GetMovieAndDealWithInvalidRespones(CancellationToken cancellationToken)
        {
            var httpClient = _httpClientFactory.CreateClient("MoviesClient");

            var request = new HttpRequestMessage(
                HttpMethod.Get,
                "api/movies/030a43b0-f9a5-405a-811c-bf342524b2be");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

            using (var response = await httpClient.SendAsync(request,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken))
            {
                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        Console.WriteLine("The requested movie cannot be found.");
                        return;
                    }
                    else if (response.StatusCode == HttpStatusCode.Unauthorized)
                        return;

                    response.EnsureSuccessStatusCode();
                }

                var stream = await response.Content.ReadAsStreamAsync(CancellationToken.None);

                var movie = stream.ReadAndDeserializeFromJson<Movie>();
            }
        }
    }
}
