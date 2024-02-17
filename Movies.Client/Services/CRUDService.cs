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
                new MediaTypeWithQualityHeaderValue("application/xml"));

        }

        public async Task Run()
        {
            await GetResource();
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
    }
}