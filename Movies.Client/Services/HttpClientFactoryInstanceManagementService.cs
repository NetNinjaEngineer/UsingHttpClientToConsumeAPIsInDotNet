using System.Net.Http.Headers;

namespace Movies.Client.Services
{
    public class HttpClientFactoryInstanceManagementService : IIntegrationService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly MovieClient _movieClient;

        public HttpClientFactoryInstanceManagementService(
            IHttpClientFactory httpClientFactory,
            MovieClient movieClient)
        {
            _httpClientFactory = httpClientFactory;
            _cancellationTokenSource = new CancellationTokenSource();
            _movieClient = movieClient;

        }


        public async Task Run()
        {
            //await GetMoviesWithHttpClientFactory(_cancellationTokenSource.Token);
            //await GetMoviesWithNamedHttpClientFactory(_cancellationTokenSource.Token);
            await GetMoviesWithTypedHttpClientFactory(_cancellationTokenSource.Token);
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

        private async Task GetMoviesWithTypedHttpClientFactory(CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get,
                "api/Movies");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using var response = await _movieClient.Client.SendAsync(request,
                HttpCompletionOption.ResponseContentRead,
                _cancellationTokenSource.Token);

            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStreamAsync();
            var movies = responseContent.ReadAndDeserializeFromJson<List<Movie>>();

        }
    }
}
