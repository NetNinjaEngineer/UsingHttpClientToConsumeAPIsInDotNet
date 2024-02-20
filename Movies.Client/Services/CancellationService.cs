using System.Net;
using System.Net.Http.Headers;

namespace Movies.Client.Services
{
    public class CancellationService : IIntegrationService
    {
        private readonly static HttpClient _httpClient = new(new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.GZip
        });

        private readonly static CancellationTokenSource _cancellationTokenSource = new();

        public CancellationService()
        {
            _httpClient.BaseAddress = new Uri("http://localhost:5137");
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.Timeout = new TimeSpan(0, 0, 2);
        }

        public async Task Run()
        {
            _cancellationTokenSource.CancelAfter(1000);
            //await GetTrailerAndCancel(_cancellationTokenSource.Token);
            await GetTrailerAndHandleTimeout();
        }

        private static async Task GetTrailerAndCancel(CancellationToken cancellationToken)
        {
            HttpRequestMessage request = new(HttpMethod.Get,
                $"api/movies/d8663e5e-7494-4f81-8739-6e0de1bea7ee/trailers/{Guid.NewGuid()}");

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

            try
            {
                using var response = await _httpClient.SendAsync(request,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken);

                response.EnsureSuccessStatusCode();

                var responseContentAsStream = await response.Content.ReadAsStreamAsync();

                var trailer = responseContentAsStream.ReadAndDeserializeFromJson<Trailer>();
            }
            catch (OperationCanceledException ex)
            {
                Console.WriteLine($"The operation was cancelled with message: {ex.Message}");
            }

        }

        private static async Task GetTrailerAndHandleTimeout()
        {
            var request = new HttpRequestMessage(HttpMethod.Get,
                $"api/movies/d8663e5e-7494-4f81-8739-6e0de1bea7ee/trailers/{Guid.NewGuid()}");

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            try
            {
                using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                var streamContent = await response.Content.ReadAsStreamAsync();
                var trailer = streamContent.ReadAndDeserializeFromJson<Trailer>();
            }
            catch (OperationCanceledException opCancelledException)
            {
                Console.WriteLine($"The operation was cancelled with message: {opCancelledException.Message}");
            }

        }
    }
}
