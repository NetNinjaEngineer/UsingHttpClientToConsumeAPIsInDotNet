using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text.Json;

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
            //await GetPosterWithStreamAndCompletionMode();
            //await TestGetPosterWithoutStream();
            //await TestGetPosterWithStream();
            await TestGetPosterWithStreamAndCompletionMode();
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

        private static async Task GetPosterWithoutStream()
        {
            var request = new HttpRequestMessage(HttpMethod.Get,
                $"api/movies/d8663e5e-7494-4f81-8739-6e0de1bea7ee/posters/{Guid.NewGuid()}");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var poster = JsonSerializer.Deserialize<Poster>(content);
        }

        private static async Task TestGetPosterWithoutStream()
        {
            await GetPosterWithoutStream();

            var stopWatch = Stopwatch.StartNew();
            for (int i = 0; i < 200; i++)
            {
                await GetPosterWithoutStream();
            }

            stopWatch.Stop();

            await Console.Out.WriteLineAsync($"EllapsedMilliseconds without stream: " +
                $"{stopWatch.ElapsedMilliseconds}, averaging = " +
                $"{stopWatch.ElapsedMilliseconds / 200} ElapsedMilliseconds/request");

        }

        private static async Task TestGetPosterWithStream()
        {
            await GetPosterWithStream();

            var stopWatch = Stopwatch.StartNew();
            for (int i = 0; i < 200; i++)
            {
                await GetPosterWithStream();
            }

            stopWatch.Stop();

            await Console.Out.WriteLineAsync($"EllapsedMilliseconds with stream: " +
                $"{stopWatch.ElapsedMilliseconds}, averaging = " +
                $"{stopWatch.ElapsedMilliseconds / 200} elapsedMilliseconds/request");

        }

        private static async Task TestGetPosterWithStreamAndCompletionMode()
        {
            await GetPosterWithStreamAndCompletionMode();

            var stopWatch = Stopwatch.StartNew();

            for (int i = 0; i < 200; i++)
                await GetPosterWithStreamAndCompletionMode();

            stopWatch.Stop();

            await Console.Out.WriteLineAsync($"EllapsedMilliseconds with stream and completionMode:  " +
             $"{stopWatch.ElapsedMilliseconds}, averaging = " +
             $"{stopWatch.ElapsedMilliseconds / 200} elapsedMilliseconds/request");

        }
    }
}
