namespace Movies.Client.Services
{
    public class CRUDService : IIntegrationService
    {
        private readonly static HttpClient _httpClient = new();

        public CRUDService()
        {
            _httpClient.BaseAddress = new Uri("https://localhost:7210");
            _httpClient.Timeout = new TimeSpan(0, 0, 30);
        }

        public async Task Run()
        {

        }
    }
}