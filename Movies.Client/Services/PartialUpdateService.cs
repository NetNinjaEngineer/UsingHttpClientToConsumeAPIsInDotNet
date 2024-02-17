using Microsoft.AspNetCore.JsonPatch;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace Movies.Client.Services
{
    public class PartialUpdateService : IIntegrationService
    {
        private readonly static HttpClient _httpClient = new();

        public PartialUpdateService()
        {
            _httpClient.BaseAddress = new Uri("https://localhost:7210");
            _httpClient.Timeout = new TimeSpan(0, 0, 30);
            _httpClient.DefaultRequestHeaders.Clear();
        }

        public async Task Run()
        {
            await PatchResource();
        }

        private static async Task PatchResource()
        {
            var patchDocument = new JsonPatchDocument<MovieForUpdate>();
            patchDocument.Replace(x => x.Title, "Updated Title");
            patchDocument.Remove(x => x.Description);

            var serializedChangeSet = JsonConvert.SerializeObject(patchDocument);
            var request = new HttpRequestMessage(HttpMethod.Patch, "api/movies/6e87f657-f2c1-4d90-9b37-cbe43cc6adb9");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Content = new StringContent(serializedChangeSet);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json-patch+json");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var updatedMovie = JsonConvert.DeserializeObject<Movie>(content);

        }

        private static async Task PatchResourceShortcut()
        {
            var patchDocument = new JsonPatchDocument<MovieForUpdate>();
            patchDocument.Replace(x => x.Title, "Updated Title");
            patchDocument.Remove(x => x.Description);

            var serializedChangeSet = JsonConvert.SerializeObject(patchDocument);

            var response = await _httpClient.PatchAsync("api/movies/6e87f657-f2c1-4d90-9b37-cbe43cc6adb9",
                new StringContent(serializedChangeSet, Encoding.UTF8, new MediaTypeHeaderValue("application/json-patch+json"));

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var updatedMovie = JsonConvert.DeserializeObject<Movie>(content);

        }
    }
}
