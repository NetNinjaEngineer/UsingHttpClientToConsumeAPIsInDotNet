using System.Net.Http.Headers;

namespace Movies.Client.Services
{
    public class HttpHandlersService : IIntegrationService
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly CancellationTokenSource cancellationTokenSource;

        public HttpHandlersService(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
            cancellationTokenSource = new CancellationTokenSource();
        }



        public async Task Run()
        {
            await GetMoviesWithRetryPolicy(cancellationTokenSource.Token);
        }

        private async Task GetMoviesWithRetryPolicy(CancellationToken cancellationToken)
        {
            var httpClient = httpClientFactory.CreateClient("MoviesClient");

            var request = new HttpRequestMessage(
                HttpMethod.Get,
                "api/movies/030a43b0-f9a5-405a-811c-bf342524b2be");

            // var request = new HttpRequestMessage(
            //     HttpMethod.Get,
            //     "api/movies/5b1c2b4d-48c7-402a-80c3-cc796ad49c6b");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

            using (var response = await httpClient.SendAsync(request,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken))
            {
                var stream = await response.Content.ReadAsStreamAsync(CancellationToken.None);

                if (!response.IsSuccessStatusCode)
                {
                    // inspect the status code
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        // show this to the user
                        Console.WriteLine("The requested movie cannot be found.");
                        return;
                    }
                    response.EnsureSuccessStatusCode();
                }
                var movie = stream.ReadAndDeserializeFromJson<Movie>();
            }
        }
    }
}
