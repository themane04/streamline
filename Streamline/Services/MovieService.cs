using System.Text.Json;
using Streamline.Utilities;
using Streamline.Models;
using System.Diagnostics;
using Streamline.Contexts;

namespace Streamline.Services;

public class MovieService
{
    private readonly string _apiKey = Environments.GetApiKey();
    private readonly string _baseUrl = Environments.GetDbUrl();

    public async Task<List<Movie>> GetPopularMoviesAsync(int page)
    {
        using HttpClient client = new();
        string url = $"{_baseUrl}movie/popular?api_key={_apiKey}&language=en-US&page={page}";

        try
        {
            HttpResponseMessage response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize(json, MovieJsonContext.Default.MovieResponse);

                if (result?.Results != null)
                {
                    return result.Results;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }

        return new List<Movie>();
    }

    public async Task<List<Movie>> SearchMoviesAsync(string query)
    {
        using HttpClient client = new();
        string url = $"{_baseUrl}search/movie?api_key={_apiKey}&language=en-US&query={Uri.EscapeDataString(query)}";

        try
        {
            HttpResponseMessage response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize(json, MovieJsonContext.Default.MovieResponse);
                if (result?.Results != null)
                {
                    return result.Results;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }

        return new List<Movie>();
    }
    
    public async Task<MovieDetail?> GetMovieDetailByIdAsync(int id)
    {
        using HttpClient client = new();
        string url = $"{_baseUrl}movie/{id}?api_key={_apiKey}&language=en-US";

        try
        {
            HttpResponseMessage response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<MovieDetail>(json);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }

        return null;
    }
}