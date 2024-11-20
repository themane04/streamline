using System.Text.Json;
using Streamline.Modules.context;
using Streamline.Modules.model;

namespace Streamline.Modules.service;

public class MovieService
{
    private static readonly string ApiKey = "d9cb9af8ce13f4a3a6a3d19dde83783f";
    private static readonly string BaseUrl = "https://api.themoviedb.org/3/";

    public static async Task<List<Movie>> GetPopularMoviesAsync()
    {
        using HttpClient client = new();
        string url = $"{BaseUrl}movie/popular?api_key={ApiKey}&language=en-US&page=1";

        try
        {
            HttpResponseMessage response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize(json, MovieJsonContext.Default.MovieResponse);
                return result?.Results ?? new List<Movie>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching movies: {ex.Message}");
        }

        return new List<Movie>();
    }
}