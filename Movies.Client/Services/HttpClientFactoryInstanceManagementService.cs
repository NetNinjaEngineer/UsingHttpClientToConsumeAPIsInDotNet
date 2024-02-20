using System.Net.Http.Headers;

namespace Movies.Client.Services
{
    public class HttpClientFactoryInstanceManagementService : IIntegrationService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public HttpClientFactoryInstanceManagementService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _cancellationTokenSource = new CancellationTokenSource();
        }


        public async Task Run()
        {
            //await GetMoviesWithHttpClientFactory(_cancellationTokenSource.Token);
            await GetMoviesWithNamedHttpClientFactory(_cancellationTokenSource.Token);
        }

        private async Task GetMoviesWithHttpClientFactory(CancellationToken cancellationToken)
        {
            var httpClient = _httpClientFactory.CreateClient();

            var request = new HttpRequestMessage(HttpMethod.Get,
                "https://localhost:7210/api/Movies");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using var response = await httpClient.SendAsync(request,
                HttpCompletionOption.ResponseContentRead,
                _cancellationTokenSource.Token);

            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStreamAsync();
            var movies = responseContent.ReadAndDeserializeFromJson<List<Movie>>();

        }

        private async Task GetMoviesWithNamedHttpClientFactory(CancellationToken cancellationToken)
        {
            var httpClient = _httpClientFactory.CreateClient("MoviesClient");

            var request = new HttpRequestMessage(HttpMethod.Get,
                "api/Movies");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using var response = await httpClient.SendAsync(request,
                HttpCompletionOption.ResponseContentRead,
                _cancellationTokenSource.Token);

            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStreamAsync();
            var movies = responseContent.ReadAndDeserializeFromJson<List<Movie>>();

        }
    }
}
