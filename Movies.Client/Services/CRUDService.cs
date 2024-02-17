using System.Net.Http.Headers;
using System.Text.Json;
using System.Xml.Serialization;

namespace Movies.Client.Services
{
    public class CRUDService : IIntegrationService
    {
        private readonly static HttpClient _httpClient = new();

        public CRUDService()
        {
            _httpClient.BaseAddress = new Uri("https://localhost:7210");
            _httpClient.Timeout = new TimeSpan(0, 0, 30);
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/xml", 0.9));

        }

        public async Task Run()
        {
            await UpdateResource();
        }

        private static async Task GetResource()
        {
            var response = await _httpClient.GetAsync("api/Movies");

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            var movies = new List<Movie>();

            if (response.Content.Headers.ContentType?.MediaType == "application/json")
            {
                movies = JsonSerializer.Deserialize<List<Movie>>(content, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
            }
            else if (response.Content.Headers.ContentType?.MediaType == "application/xml")
            {
                var xmlSerializer = new XmlSerializer(typeof(List<Movie>));

                movies = xmlSerializer.Deserialize(new StringReader(content)) as List<Movie>;

            }



        }

        private static async Task GetResourceUsingHttpRequestMessage()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/Movies");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var movies = new List<Movie>();

            if (response.Content.Headers.ContentType?.MediaType == "application/json")
                movies = JsonSerializer.Deserialize<List<Movie>>(content, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
            else if (response.Content.Headers.ContentType?.MediaType == "application/xml")
            {
                var xmlSerializer = new XmlSerializer(typeof(List<Movie>));
                movies = xmlSerializer.Deserialize(new StringReader(content)) as List<Movie>;
            }

        }

        private static async Task CreateResource()
        {
            var movieToCreate = new MovieForCreation()
            {
                Title = "Reservoir Dogs",
                Description = "After a simple jewelry heist goes terribly wrong, the " +
                 "surviving criminals begin to suspect that one of them is a police informant.",
                DirectorId = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b35"),
                ReleaseDate = new DateTimeOffset(new DateTime(1992, 9, 2)),
                Genre = "Crime, Drama"
            };

            var serializedMovie = JsonSerializer.Serialize(movieToCreate);
            var request = new HttpRequestMessage(HttpMethod.Post, "api/Movies");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Content = new StringContent(serializedMovie);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            var createdMovie = JsonSerializer.Deserialize<Movie>(content, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

        }

        private static async Task UpdateResource()
        {
            var movieToUpdate = new MovieForUpdate()
            {
                Title = "Pulp Fiction",
                Description = "The movie with Zed.",
                DirectorId = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b35"),
                ReleaseDate = new DateTimeOffset(new DateTime(1992, 9, 2)),
                Genre = "Crime, Drama"
            };

            var serializedMovieToUpdate = JsonSerializer.Serialize(movieToUpdate);

            var request = new HttpRequestMessage(HttpMethod.Put,
                "api/movies/5b1c2b4d-48c7-402a-80c3-cc796ad49c6b");

            request.Content = new StringContent(serializedMovieToUpdate);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var content = await request.Content.ReadAsStringAsync();

            var updatedMovie = JsonSerializer.Deserialize<Movie>(content);

        }
    }
}