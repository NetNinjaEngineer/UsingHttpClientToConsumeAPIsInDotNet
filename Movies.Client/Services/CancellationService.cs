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
            _httpClient.Timeout = new TimeSpan(0, 0, 30);
        }

        public async Task Run()
        {
            _cancellationTokenSource.CancelAfter(1000);
            await GetTrailerAndCancel(_cancellationTokenSource.Token);
        }

        private static async Task GetTrailerAndCancel(CancellationToken cancellationToken)
        {
            HttpRequestMessage request = new(HttpMethod.Get,
                $"api/movies/d8663e5e-7494-4f81-8739-6e0de1bea7ee/trailers/{Guid.NewGuid()}");

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

            using var response = await _httpClient.SendAsync(request,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken);
            
            response.EnsureSuccessStatusCode();

            var responseContentAsStream = await response.Content.ReadAsStreamAsync();

            var trailer = responseContentAsStream.ReadAndDeserializeFromJson<Trailer>();

        }
    }
}
