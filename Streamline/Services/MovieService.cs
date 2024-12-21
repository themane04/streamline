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
                var result = JsonSerializer.Deserialize(json, MovieJsonContext.Default.MovieResponseTmdb);

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
                var result = JsonSerializer.Deserialize(json, MovieJsonContext.Default.MovieResponseTmdb);
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
                var result = JsonSerializer.Deserialize(json, MovieJsonContext.Default.MovieResponseTmdb);

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

    public async Task AddOrUpdateMovieAndAddToWatchlist(MovieShort movie)
    {
        string methodName = nameof(AddOrUpdateMovieAndAddToWatchlist);
        _logger.LogInformation($"{methodName}: Starting process to add or update movie and add it to watchlist");

        try
        {
            var (isSuccess, alreadyExists) = await AddOrUpdateMovie(movie);

            if (isSuccess || alreadyExists)
            {
                if (alreadyExists)
                {
                    _logger.LogInformation($"{methodName}: Movie already exists. Proceeding to add to watchlist.");
                }

                var addToWatchlistResponse = await _httpClient.PostAsync(
                    $"{_backendApiUrl}{BackendEndpoints.Movies}/{movie.MovieId}/add_to_watchlist", null);

                if (addToWatchlistResponse.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"{methodName}: Movie successfully added to watchlist");
                }
                else
                {
                    _logger.LogError(
                        $"{methodName}: Failed to add movie to watchlist with status: {addToWatchlistResponse.StatusCode}");
                    addToWatchlistResponse.EnsureSuccessStatusCode();
                }
            }
            else
            {
                _logger.LogError($"{methodName}: Failed to add movie with unknown issue.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{methodName}: Exception occurred during the process");
            throw;
        }
    }

    public async Task AddOrUpdateMovieAndMarkAsFavorite(MovieShort movie)
    {
        string methodName = nameof(AddOrUpdateMovieAndMarkAsFavorite);
        _logger.LogInformation($"{methodName}: Starting process to add or update movie and mark it as favorite");

        try
        {
            var (isSuccess, alreadyExists) = await AddOrUpdateMovie(movie);

            if (isSuccess || alreadyExists)
            {
                if (alreadyExists)
                {
                    _logger.LogInformation($"{methodName}: Movie already exists. Proceeding to mark as favorite.");
                }

                var markFavoriteResponse = await _httpClient.PostAsync(
                    $"{_backendApiUrl}{BackendEndpoints.Movies}/{movie.MovieId}/mark_favorite", null);

                if (markFavoriteResponse.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"{methodName}: Movie successfully marked as favorite");
                }
                else
                {
                    _logger.LogError(
                        $"{methodName}: Failed to mark movie as favorite with status: {markFavoriteResponse.StatusCode}");
                    markFavoriteResponse.EnsureSuccessStatusCode();
                }
            }
            else
            {
                _logger.LogError($"{methodName}: Failed to add movie with unknown issue");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{methodName}: Exception occurred during the process");
            throw;
        }
    }

    public async Task<List<MovieShort>> GetWatchlist()
    {
        string methodName = nameof(GetWatchlist);
        _logger.LogInformation($"{methodName}: Retrieving the watchlist");

        try
        {
            var response = await _httpClient.GetAsync($"{_backendApiUrl}{BackendEndpoints.Movies}?watchlist=true");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"{methodName}: Failed to retrieve watchlist with status code {response.StatusCode}");
                return new List<MovieShort>();
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(jsonResponse))
            {
                _logger.LogWarning($"{methodName}: No movies found in response.");
                return new List<MovieShort>();
            }

            try
            {
                if (jsonResponse.StartsWith("["))
                {
                    var movies = JsonSerializer.Deserialize<List<MovieShort>>(jsonResponse,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return movies ?? new List<MovieShort>();
                }

                var structuredResponse = JsonSerializer.Deserialize<MovieResponse<List<MovieShort>>>(jsonResponse,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (structuredResponse?.Data != null)
                {
                    return structuredResponse.Data;
                }

                _logger.LogWarning($"{methodName}: No movies found.");
                return new List<MovieShort>();
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, $"{methodName}: Error parsing JSON response");
                return new List<MovieShort>();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{methodName}: Exception occurred while retrieving movies from the watchlist");
            throw;
        }
    }

    public async Task<List<MovieShort>> GetFavorites()
    {
        string methodName = nameof(GetFavorites);
        _logger.LogInformation($"{methodName}: Retrieving the favorites");

        try
        {
            var response = await _httpClient.GetAsync($"{_backendApiUrl}{BackendEndpoints.Movies}?favorites=true");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"{methodName}: Failed to retrieve favorites with status code {response.StatusCode}");
                return new List<MovieShort>();
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(jsonResponse))
            {
                _logger.LogWarning($"{methodName}: No favorite movies found in response.");
                return new List<MovieShort>();
            }

            try
            {
                if (jsonResponse.StartsWith("["))
                {
                    var movies = JsonSerializer.Deserialize<List<MovieShort>>(jsonResponse,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    return movies ?? new List<MovieShort>();
                }

                var structuredResponse = JsonSerializer.Deserialize<MovieResponse<List<MovieShort>>>(jsonResponse,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (structuredResponse?.Data != null)
                {
                    return structuredResponse.Data;
                }

                _logger.LogWarning($"{methodName}: No favorite movies found.");
                return new List<MovieShort>();
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, $"{methodName}: Error parsing JSON response");
                return new List<MovieShort>();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{methodName}: Exception occurred while retrieving the favorites");
            throw;
        }
    }

    public Task<bool> IsMovieInWatchlist(int movieId)
    {
        return IsMoviePropertyTrue(movieId, "IsWatchlisted");
    }

    public Task<bool> IsMovieInFavorites(int movieId)
    {
        return IsMoviePropertyTrue(movieId, "IsFavorite");
    }

    public async Task RemoveFromWatchlist(int movieId)
    {
        string methodName = nameof(RemoveFromWatchlist);
        _logger.LogInformation($"{methodName}: Removing movie with MovieID: {movieId} from the watchlist");

        try
        {
            var response =
                await _httpClient.PostAsync(
                    $"{_backendApiUrl}{BackendEndpoints.Movies}/{movieId}/remove_from_watchlist",
                    null);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation(
                    $"{methodName}: Movie with MovieID: {movieId} successfully removed from the watchlist");
            }
            else
            {
                _logger.LogError(
                    $"{methodName}: Failed to remove movie with MovieID: {movieId} from the watchlist, status code: {response.StatusCode}");
                response.EnsureSuccessStatusCode();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                $"{methodName}: Exception occurred while removing movie with MovieID: {movieId} from the watchlist");
            throw;
        }
    }

    public async Task RemoveFromFavorites(int movieId)
    {
        string methodName = nameof(RemoveFromFavorites);
        _logger.LogInformation($"{methodName}: Removing movie with MovieID: {movieId} from the favorites");

        try
        {
            var response =
                await _httpClient.PostAsync(
                    $"{_backendApiUrl}{BackendEndpoints.Movies}/{movieId}/unmark_favorite",
                    null);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation(
                    $"{methodName}: Movie with MovieID: {movieId} successfully removed from the favorites");
            }
            else
            {
                _logger.LogError(
                    $"{methodName}: Failed to remove movie with MovieID: {movieId} from the favorites, status code: {response.StatusCode}");
                response.EnsureSuccessStatusCode();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                $"{methodName}: Exception occurred while removing movie with MovieID: {movieId} from the favorites");
            throw;
        }
    }

    private async Task<(bool isSuccess, bool alreadyExists)> AddOrUpdateMovie(MovieShort movie)
    {
        var json = JsonSerializer.Serialize(movie);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync($"{_backendApiUrl}{BackendEndpoints.Movies}", content);

        if (response.IsSuccessStatusCode)
        {
            return (true, false);
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            try
            {
                var errorResponse = JsonSerializer.Deserialize<MovieResponseNoInterface>(responseContent);
                if (errorResponse?.Message.Contains("already exists") == true)
                {
                    return (false, true);
                }
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Exception occurred while adding or updating a movie");
            }
        }

        return (false, false);
    }

    public async Task<bool> IsMoviePropertyTrue(int movieId, string propertyName)
    {
        string methodName = $"IsMovie{propertyName}True";
        _logger.LogInformation($"{methodName}: Checking if movie with MovieID: {movieId} has {propertyName} true");

        try
        {
            var response = await _httpClient.GetAsync($"{_backendApiUrl}{BackendEndpoints.Movies}/{movieId}");
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var movie = JsonSerializer.Deserialize<MovieShort>(responseContent);

                var propertyValue = movie?.GetType().GetProperty(propertyName)?.GetValue(movie, null);
                bool isPropertyTrue = propertyValue is bool boolValue && boolValue;

                if (isPropertyTrue)
                {
                    _logger.LogInformation($"{methodName}: Movie with MovieID: {movieId} has {propertyName} true");
                    return true;
                }
                else
                {
                    _logger.LogInformation(
                        $"{methodName}: Movie with MovieID: {movieId} does not have {propertyName} true");
                    return false;
                }
            }

            _logger.LogError(
                $"{methodName}: Failed to fetch movie with MovieID: {movieId}, status code: {response.StatusCode}");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                $"{methodName}: Exception occurred while checking if movie with MovieID: {movieId} has {propertyName} true");
            throw;
        }
    }
}