using System.Text.Json;
using Streamline.Utilities;
using Streamline.Models;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Streamline.Contexts;

namespace Streamline.Services;

public class MovieService
{
    private readonly string _apiKey = Environments.GetApiKey();
    private readonly string _baseUrl = Environments.GetDbUrl();
    private readonly MovieDbContext _dbContext;

    public MovieService(MovieDbContext dbContext)
    {
        _dbContext = dbContext;
    }

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

    public async Task<List<MovieDetail>> GetSimilarMoviesAsync(int movieId)
    {
        using HttpClient client = new();
        string url = $"{_baseUrl}movie/{movieId}/similar?api_key={_apiKey}&language=en-US";

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
        _dbContext.WatchlistMovies.Add(movie);
        await _dbContext.SaveChangesAsync();
    }
    
    public async Task<List<MovieWatchlist>> GetWatchlist()
    {
        return await _dbContext.WatchlistMovies.ToListAsync();
    }
    
    public async Task<bool> IsMovieInWatchlist(int movieId)
    {
        return await _dbContext.WatchlistMovies.AnyAsync(m => m.MovieId == movieId);
    }
}