using System.Net.Http.Headers;

namespace Movies.Client;

public class MovieClient
{
    private readonly HttpClient _httpClient;

    public MovieClient(HttpClient client)
    {
        _httpClient = client;
        _httpClient.BaseAddress = new Uri("https://localhost:7210");
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.Timeout = new TimeSpan(0, 0, 30);
    }

    public async Task<IEnumerable<Movie>> GetMoviesAsync(CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get,
            "api/Movies");
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        using var response = await _httpClient.SendAsync(request,
            HttpCompletionOption.ResponseContentRead,
            cancellationToken);

        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStreamAsync();
        var movies = responseContent.ReadAndDeserializeFromJson<List<Movie>>();
        return movies;
    }
}
