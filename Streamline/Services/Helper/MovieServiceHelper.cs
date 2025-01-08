using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Streamline.Models;
using Streamline.Utilities;

namespace Streamline.Services.Helper;

public class MovieServiceHelper
{
    private readonly string _backendApiUrl = Environments.GetBackendApiUrl();
    private readonly HttpClient _httpClient;
    private readonly ILogger<MovieService> _logger;

    public MovieServiceHelper(HttpClient httpClient, ILogger<MovieService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    private void SetAuthorizationHeader(string? accessToken)
    {
        if (!string.IsNullOrEmpty(accessToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        }
        else
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }

    public async Task PerformActionAfterAddOrUpdateMovie(string methodName, MovieShort movie,
        Func<Task<HttpResponseMessage>> action, string successMessage, string failureMessage)
    {
        try
        {
            var (isSuccess, alreadyExists) = await AddOrUpdateMovie(movie);

            if (isSuccess || alreadyExists)
            {
                if (alreadyExists)
                {
                    _logger.LogInformation($"{methodName}: Movie already exists. Proceeding to next step.");
                }

                var response = await action();

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"{methodName}: {successMessage}");
                }
                else
                {
                    _logger.LogError($"{methodName}: {failureMessage} with status: {response.StatusCode}");
                    response
                        .EnsureSuccessStatusCode();
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

    public async Task RemoveMovie(string endpointSuffix, int movieId, string methodName)
    {
        _logger.LogInformation($"{methodName}: Removing movie with MovieID: {movieId}");

        try
        {
            var accessToken = await SecureStorage.GetAsync("accessToken");
            SetAuthorizationHeader(accessToken);

            var response = await _httpClient.PostAsync($"{_backendApiUrl}{endpointSuffix}", null);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation($"{methodName}: Movie with MovieID: {movieId} successfully removed");
            }
            else
            {
                _logger.LogError(
                    $"{methodName}: Failed to remove movie with MovieID: {movieId}, status code: {response.StatusCode}");
                response.EnsureSuccessStatusCode();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{methodName}: Exception occurred while removing movie with MovieID: {movieId}");
            throw;
        }
    }

    public async Task<List<MovieShort>> GetMovieList(string urlSuffix, string methodName)
    {
        _logger.LogInformation($"{methodName}: Retrieving data");

        try
        {
            var accessToken = await SecureStorage.GetAsync("accessToken");
            SetAuthorizationHeader(accessToken);

            var response = await _httpClient.GetAsync($"{_backendApiUrl}{urlSuffix}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"{methodName}: Failed to retrieve data with status code {response.StatusCode}");
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

                var structuredResponse = JsonSerializer.Deserialize<BackendResponse<List<MovieShort>>>(
                    jsonResponse,
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
            _logger.LogError(ex, $"{methodName}: Exception occurred while retrieving data");
            throw;
        }
    }

    private async Task<(bool isSuccess, bool alreadyExists)> AddOrUpdateMovie(MovieShort movie)
    {
        var json = JsonSerializer.Serialize(movie);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var accessToken = await SecureStorage.GetAsync("accessToken");
        SetAuthorizationHeader(accessToken);

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
                var errorResponse = JsonSerializer.Deserialize<MovieResponseMessage>(responseContent);
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
            var accessToken = await SecureStorage.GetAsync("accessToken");
            SetAuthorizationHeader(accessToken);

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

                _logger.LogInformation(
                    $"{methodName}: Movie with MovieID: {movieId} does not have {propertyName} true");
                return false;
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