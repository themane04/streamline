using System.Text.Json;
using Streamline.Utilities;
using Streamline.Models;
using Microsoft.Extensions.Logging;
using Streamline.Contexts;
using Streamline.Services.Helper;

namespace Streamline.Services;

public class MovieService
{
    private readonly string _apiKey = Environments.GetApiKey();
    private readonly string _movieDbApiUrl = Environments.MovieDbApiUrl();
    private readonly string _backendApiUrl = Environments.GetBackendApiUrl();
    private readonly HttpClient _httpClient;
    private readonly ILogger<MovieService> _logger;
    private readonly MovieServiceHelper _movieServiceHelper;

    public MovieService(HttpClient httpClient, ILogger<MovieService> logger, MovieServiceHelper movieServiceHelper)
    {
        _httpClient = httpClient;
        _logger = logger;
        _movieServiceHelper = movieServiceHelper;
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
        await _movieServiceHelper.PerformActionAfterAddOrUpdateMovie(nameof(AddOrUpdateMovieAndAddToWatchlist), movie, () =>
                _httpClient.PostAsync($"{_backendApiUrl}{BackendEndpoints.Movies}/{movie.MovieId}/add_to_watchlist",
                    null),
            "Movie successfully added to watchlist", "Failed to add movie to watchlist");
    }

    public async Task AddOrUpdateMovieAndMarkAsFavorite(MovieShort movie)
    {
        await _movieServiceHelper.PerformActionAfterAddOrUpdateMovie(nameof(AddOrUpdateMovieAndMarkAsFavorite), movie, () =>
                _httpClient.PostAsync($"{_backendApiUrl}{BackendEndpoints.Movies}/{movie.MovieId}/mark_favorite",
                    null),
            "Movie successfully marked as favorite", "Failed to mark movie as favorite");
    }

    public async Task<List<MovieShort>> GetWatchlist()
    {
        return await _movieServiceHelper.GetMovieList($"{BackendEndpoints.Movies}?watchlist=true", nameof(GetWatchlist));
    }

    public async Task<List<MovieShort>> GetFavorites()
    {
        return await _movieServiceHelper.GetMovieList($"{BackendEndpoints.Movies}?favorites=true", nameof(GetFavorites));
    }

    public Task<BackendResponseNullableData<MovieShort>> IsMovieInWatchlist(int movieId)
    {
        return _movieServiceHelper.IsMoviePropertyTrue(movieId, "IsWatchlisted");
    }

    public Task<BackendResponseNullableData<MovieShort>> IsMovieInFavorites(int movieId)
    {
        return _movieServiceHelper.IsMoviePropertyTrue(movieId, "IsFavorite");
    }

    public async Task RemoveFromWatchlist(int movieId)
    {
        await _movieServiceHelper.RemoveMovie($"{BackendEndpoints.Movies}/{movieId}/remove_from_watchlist", movieId,
            nameof(RemoveFromWatchlist));
    }

    public async Task RemoveFromFavorites(int movieId)
    {
        await _movieServiceHelper.RemoveMovie($"{BackendEndpoints.Movies}/{movieId}/unmark_favorite", movieId,
            nameof(RemoveFromFavorites));
    }
}