using System.Text.Json;
using Streamline.Utilities;
using Streamline.Models;
using System.Diagnostics;
using System.Net;
using System.Text;
using Streamline.Contexts;

namespace Streamline.Services;

public class MovieService
{
    private readonly string _apiKey = Environments.GetApiKey();
    private readonly string _movieDbApiUrl = Environments.MovieDbApiUrl();
    private readonly string _backendApiUrl = Environments.GetBackendApiUrl();
    private readonly HttpClient _httpClient;

    public MovieService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Movie>> GetPopularMoviesAsync(int page)
    {
        using HttpClient client = new();
        string url = $"{_movieDbApiUrl}movie/popular?api_key={_apiKey}&language=en-US&page={page}";

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
        string url =
            $"{_movieDbApiUrl}search/movie?api_key={_apiKey}&language=en-US&query={Uri.EscapeDataString(query)}";

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
        string url = $"{_movieDbApiUrl}movie/{id}?api_key={_apiKey}&language=en-US";

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

    public async Task<List<MovieDetail>> GetSimilarMoviesAsync(int movieId)
    {
        using HttpClient client = new();
        string url = $"{_movieDbApiUrl}movie/{movieId}/similar?api_key={_apiKey}&language=en-US";

        try
        {
            HttpResponseMessage response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize(json, MovieJsonContext.Default.MovieResponse);

                if (result?.Results != null)
                {
                    List<MovieDetail> similarMovies = new List<MovieDetail>();

                    foreach (var movie in result.Results)
                    {
                        MovieDetail? movieDetail = await GetMovieDetailByIdAsync(movie.Id);
                        if (movieDetail != null)
                        {
                            similarMovies.Add(movieDetail);
                        }
                    }

                    return similarMovies;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }

        return new List<MovieDetail>();
    }

    public async Task AddToWatchlist(MovieWatchlist movie)
    {
        var json = JsonSerializer.Serialize(movie);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        Console.WriteLine("Content sending to the backend: " + content);
        var response = await _httpClient.PostAsync($"{_backendApiUrl}{BackendEndpoints.MovieEndpoint}", content);
        response.EnsureSuccessStatusCode();

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine("Failed to add movie to watchlist");
        }
        else
        {
            Console.WriteLine("Movie added to watchlist");
        }
    }

    public async Task<List<MovieWatchlist>> GetWatchlist()
    {
        var response = await _httpClient.GetAsync($"{_backendApiUrl}{BackendEndpoints.MovieEndpoint}");
        response.EnsureSuccessStatusCode();

        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<MovieWatchlist>>(json) ?? new List<MovieWatchlist>();
        }

        return new List<MovieWatchlist>();
    }

    public async Task<bool> IsMovieInWatchlist(int movieId)
    {
        var response = await _httpClient.GetAsync($"{_backendApiUrl}{BackendEndpoints.MovieEndpoint}/{movieId}");

        if (response.IsSuccessStatusCode)
        {
            return true;
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }

        throw new HttpRequestException($"Unexpected status code: {response.StatusCode}");
    }

    public async Task RemoveFromWatchlist(int movieId)
    {
        var response = await _httpClient.DeleteAsync($"{_backendApiUrl}{BackendEndpoints.MovieEndpoint}/{movieId}");
        response.EnsureSuccessStatusCode();
        Console.WriteLine(response);

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine("Failed to remove movie from watchlist");
        }
        else
        {
            Console.WriteLine("Movie removed from watchlist");
        }
    }
}