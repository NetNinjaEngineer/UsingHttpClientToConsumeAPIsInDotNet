namespace Movies.Client;

public class MovieClient
{
    public HttpClient Client { get; }

    public MovieClient(HttpClient client)
    {
        Client = client;
    }
}
