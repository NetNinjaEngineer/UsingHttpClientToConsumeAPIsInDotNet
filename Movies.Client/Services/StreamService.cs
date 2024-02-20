using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;

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
            //await TestGetPosterWithStreamAndCompletionMode();
            //await PostPosterWithStream();
            await PostAndReadPosterWithStreams();
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
            var poster = System.Text.Json.JsonSerializer.Deserialize<Poster>(content);
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

        private static async Task PostPosterWithStream()
        {
            var random = new Random();
            var generatedBytes = new byte[524288];
            random.NextBytes(generatedBytes);

            var posterForCreation = new PosterForCreation
            {
                Name = "a new poster for creation",
                Bytes = generatedBytes
            };

            using var memoryContentStream = new MemoryStream();
            memoryContentStream.SerializeAndWrite(posterForCreation);

            memoryContentStream.Seek(0, SeekOrigin.Begin);

            using HttpRequestMessage requestMessage = new(HttpMethod.Post,
                "api/movies/d8663e5e-7494-4f81-8739-6e0de1bea7ee/posters");

            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using StreamContent streamContent = new(memoryContentStream);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            requestMessage.Content = streamContent;

            HttpResponseMessage responseMessage = await _httpClient.SendAsync(requestMessage);

            responseMessage.EnsureSuccessStatusCode();

            var createdPoster = JsonConvert.DeserializeObject<Poster>(
                await responseMessage.Content.ReadAsStringAsync());

        }

        private static async Task PostAndReadPosterWithStreams()
        {
            var random = new Random();
            var generatedBytes = new byte[524288];
            random.NextBytes(generatedBytes);

            var posterForCreation = new PosterForCreation
            {
                Name = "a new poster for creation",
                Bytes = generatedBytes
            };

            var memoryContentStream = new MemoryStream();
            memoryContentStream.SerializeAndWrite(posterForCreation);

            memoryContentStream.Seek(0, SeekOrigin.Begin);

            using HttpRequestMessage requestMessage = new(HttpMethod.Post,
                "api/movies/d8663e5e-7494-4f81-8739-6e0de1bea7ee/posters");
            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue($"application/json"));
            using StreamContent streamContent = new(memoryContentStream);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            requestMessage.Content = streamContent;
            HttpResponseMessage responseMessage = await _httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead);
            Stream content = await responseMessage.Content.ReadAsStreamAsync();
            var createdPoster = content.ReadAndDeserializeFromJson<Poster>();


        }

        private static async Task PostPosterWithoutStream()
        {
            var random = new Random();
            var generatedBytes = new byte[524288];
            random.NextBytes(generatedBytes);

            var posterForCreation = new PosterForCreation
            {
                Name = "a new poster for creation",
                Bytes = generatedBytes
            };

            using var requestMessage = new HttpRequestMessage(HttpMethod.Post,
                "api/movies/d8663e5e-7494-4f81-8739-6e0de1bea7ee/posters");
            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var serializedPoster = System.Text.Json.JsonSerializer.Serialize(posterForCreation);

            requestMessage.Content = new StringContent(serializedPoster, Encoding.UTF8);

            requestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var responseMessage = await _httpClient.SendAsync(requestMessage);

            var createdPoster = System.Text.Json.JsonSerializer.Deserialize<Poster>(
                await responseMessage.Content.ReadAsStringAsync());

        }

        private static async Task TestPostPosterWithoutStream()
        {
            await PostPosterWithoutStream();

            var stopWatch = Stopwatch.StartNew();

            for (int i = 0; i < 200; i++)
                await PostPosterWithoutStream();

            stopWatch.Stop();

            await Console.Out.WriteLineAsync($"EllapsedMilliseconds without stream:  " +
                $"{stopWatch.ElapsedMilliseconds}, averaging = " +
                $"{stopWatch.ElapsedMilliseconds / 200} elapsedMilliseconds/request");

        }

        private static async Task TestPostPosterWithStream()
        {
            await PostPosterWithStream();

            var stopWatch = Stopwatch.StartNew();

            for (int i = 0; i < 200; i++)
                await PostPosterWithStream();

            stopWatch.Stop();

            await Console.Out.WriteLineAsync($"EllapsedMilliseconds with stream and completionMode:  " +
                $"{stopWatch.ElapsedMilliseconds}, averaging = " +
                $"{stopWatch.ElapsedMilliseconds / 200} elapsedMilliseconds/request");

        }
    }
}
