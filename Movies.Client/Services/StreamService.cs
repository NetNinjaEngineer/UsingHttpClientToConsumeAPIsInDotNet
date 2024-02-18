using System.Net.Http.Headers;

namespace Movies.Client.Services
{
    public class StreamService : IIntegrationService
    {
        private readonly static HttpClient _httpClient = new();

        public StreamService()
        {
            _httpClient.BaseAddress = new Uri("https://localhost:7210");
            _httpClient.Timeout = new TimeSpan(0, 0, 30);
            _httpClient.DefaultRequestHeaders.Clear();

        }

        public async Task Run()
        {
            //await GetPosterWithStream();
            await GetPosterWithStreamAndCompletionMode();
        }

        private static async Task GetPosterWithStream()
        {
            var request = new HttpRequestMessage(
                 HttpMethod.Get,
                 $"api/movies/d8663e5e-7494-4f81-8739-6e0de1bea7ee/posters/{Guid.NewGuid()}");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            using (var stream = await response.Content.ReadAsStreamAsync())
            {
                var poster = stream.ReadAndDeserializeFromJson<Poster>();
            };
        }

        private static async Task GetPosterWithStreamAndCompletionMode()
        {
            var request = new HttpRequestMessage(
                 HttpMethod.Get,
                 $"api/movies/d8663e5e-7494-4f81-8739-6e0de1bea7ee/posters/{Guid.NewGuid()}");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            using (var stream = await response.Content.ReadAsStreamAsync())
            {
                var poster = stream.ReadAndDeserializeFromJson<Poster>();
            };

        }
    }
}
