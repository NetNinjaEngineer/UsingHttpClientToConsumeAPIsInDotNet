using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Movies.Client.Services
{
    public class StreamService : IIntegrationService
    {
        private readonly static HttpClient _httpClient = new();

        public StreamService()
        {
            _httpClient.BaseAddress = new Uri("");
            _httpClient.Timeout = new TimeSpan(0, 0, 30);
            _httpClient.DefaultRequestHeaders.Clear();

        }

        public async Task Run()
        {
            await GetPosterWithStream();
        }

        private static async Task GetPosterWithStream()
        {
            var request = new HttpRequestMessage(
                 HttpMethod.Get,
                 $"api/movies/d8663e5e-7494-4f81-8739-6e0de1bea7ee/posters/{Guid.NewGuid()}");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync();

            using var streamReader = new StreamReader(stream);
            using var jsonTextReader = new JsonTextReader(streamReader);
            var jsonSerializer = new JsonSerializer();
            var poster = jsonSerializer.Deserialize<Poster>(jsonTextReader);

        }
    }
}
