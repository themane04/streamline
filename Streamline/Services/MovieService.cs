using System.Text.Json;
using Streamline.Utilities;
using Streamline.Models;
using System.Net;
using System.Text;
using Microsoft.Extensions.Logging;
using Streamline.Contexts;

namespace Streamline.Services;

public class MovieService
{
    private readonly string _apiKey = Environments.GetApiKey();
    private readonly string _movieDbApiUrl = Environments.MovieDbApiUrl();
    private readonly string _backendApiUrl = Environments.GetBackendApiUrl();
    private readonly HttpClient _httpClient;
    private readonly ILogger<MovieService> _logger;

    public MovieService(HttpClient httpClient, ILogger<MovieService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _logger.LogInformation("Initialized");
    }

    public async Task<List<Movie>> GetPopularMoviesAsync(int page)
    {
        string methodName = nameof(GetPopularMoviesAsync);
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
                    _logger.LogInformation($"{methodName}: Got {result.Results.Count} popular movies for page {page}");
                    return result.Results;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{methodName}: Failed to get popular movies");
            throw;
        }

        return new List<Movie>();
    }

    public async Task<List<Movie>> SearchMoviesAsync(string query)
    {
        string methodName = nameof(SearchMoviesAsync);
        using HttpClient client = new();
        string url =
            $"{_movieDbApiUrl}search/movie?api_key={_apiKey}&language=en-US&query={Uri.EscapeDataString(query)}";
        _logger.LogInformation($"{methodName}: Searching movies with query: {query}");

        try
        {
            HttpResponseMessage response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize(json, MovieJsonContext.Default.MovieResponse);
                if (result?.Results != null)
                {
                    _logger.LogInformation($"{methodName}: Got {result.Results.Count} search results");
                    return result.Results;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{methodName}: Failed to search movies");
            throw;
        }

        return new List<Movie>();
    }

    public async Task<MovieDetail?> GetMovieDetailByIdAsync(int id)
    {
        string methodName = nameof(GetMovieDetailByIdAsync);
        using HttpClient client = new();
        string url = $"{_movieDbApiUrl}movie/{id}?api_key={_apiKey}&language=en-US";
        _logger.LogInformation($"{methodName}: Getting movie detail for movie with MovieID: {id}");

        try
        {
            HttpResponseMessage response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"{methodName}: Got movie detail for movie with MovieID: {id}");
                return JsonSerializer.Deserialize<MovieDetail>(json);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{methodName}: Failed to get movie detail");
            throw;
        }

        return null;
    }

    public async Task<List<MovieDetail>> GetSimilarMoviesAsync(int movieId)
    {
        string methodName = nameof(GetSimilarMoviesAsync);
        using HttpClient client = new();
        string url = $"{_movieDbApiUrl}movie/{movieId}/similar?api_key={_apiKey}&language=en-US";
        _logger.LogInformation($"{methodName}: Getting similar movies for movie with MovieID: {movieId}");

        try
        {
            HttpResponseMessage response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize(json, MovieJsonContext.Default.MovieResponse);

                if (result?.Results != null)
                {
                    _logger.LogInformation($"{methodName}: Got {result.Results.Count} similar movies");
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
            _logger.LogError(ex, $"{methodName}: Failed to get similar movies");
            throw;
        }

        return new List<MovieDetail>();
    }

    public async Task AddToWatchlist(MovieWatchlist movie)
    {
        string methodName = nameof(AddToWatchlist);
        _logger.LogInformation($"{methodName}: Adding movie to watchlist");

        try
        {
            var json = JsonSerializer.Serialize(movie);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_backendApiUrl}{BackendEndpoints.Watchlist}", content);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation($"{methodName}: Movie successfully added to watchlist");
            }
            else
            {
                _logger.LogWarning($"{methodName}: Failed to add movie to watchlist: {response.StatusCode}");
                response.EnsureSuccessStatusCode();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{methodName}: Exception occurred while adding movie to watchlist");
            throw;
        }
    }

    public async Task<List<MovieWatchlist>> GetWatchlist()
    {
        string methodName = nameof(GetWatchlist);
        _logger.LogInformation($"{methodName}: Retrieving the watchlist");

        try
        {
            var response = await _httpClient.GetAsync($"{_backendApiUrl}{BackendEndpoints.Watchlist}");

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                _logger.LogInformation($"{methodName}: Watchlist is empty");
                return new List<MovieWatchlist>();
            }

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                var watchlist = JsonSerializer.Deserialize<List<MovieWatchlist>>(json) ?? new List<MovieWatchlist>();
                _logger.LogInformation($"{methodName}: Retrieved {watchlist.Count} movies in the watchlist");
                return watchlist;
            }

            _logger.LogWarning($"{methodName}: Failed to retrieve watchlist: {response.StatusCode}");
            return new List<MovieWatchlist>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{methodName}: Exception occurred while retrieving the watchlist");
            throw;
        }
    }

    public async Task<bool> IsMovieInWatchlist(int movieId)
    {
        string methodName = nameof(IsMovieInWatchlist);
        _logger.LogInformation($"{methodName}: Checking if movie with MovieID: {movieId} is in the watchlist");

        try
        {
            var response = await _httpClient.GetAsync($"{_backendApiUrl}{BackendEndpoints.Watchlist}/{movieId}");

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation($"{methodName}: Movie with MovieID: {movieId} is in the watchlist");
                return true;
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogInformation($"{methodName}: Movie with MovieID: {movieId} is not in the watchlist");
                return false;
            }

            _logger.LogError(
                $"{methodName}: Unexpected status code {response.StatusCode} when checking movie with MovieID: {movieId} in watchlist");
            throw new HttpRequestException($"Unexpected status code: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                $"{methodName}: Exception occurred while checking if movie with MovieID: {movieId} is in the watchlist");
            throw;
        }
    }

    public async Task RemoveFromWatchlist(int movieId)
    {
        string methodName = nameof(RemoveFromWatchlist);
        _logger.LogInformation($"{methodName}: Removing movie with MovieID: {movieId} from the watchlist");

        try
        {
            var response = await _httpClient.DeleteAsync($"{_backendApiUrl}{BackendEndpoints.Watchlist}/{movieId}");
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation(
                    $"{methodName}: Movie with MovieID: {movieId} successfully removed from watchlist");
            }
            else
            {
                _logger.LogWarning(
                    $"{methodName}: Failed to remove movie with MovieID: {movieId} from watchlist: {response.StatusCode}");
                response.EnsureSuccessStatusCode();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Exception occurred while removing movie with MovieID: {movieId} from the watchlist");
            throw;
        }
    }
}