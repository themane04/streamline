using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Streamline.Models;
using Streamline.Utilities;

namespace Streamline.Services;

public class UserService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<UserService> _logger;

    public UserService(HttpClient httpClient, ILogger<UserService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<AuthenticatedUser?> SignUpUserAsync(string username, string email, string password,
        string confirmPassword)
    {
        var userData = new
        {
            username,
            email,
            password,
            confirm_password = confirmPassword
        };

        var jsonRequest = JsonSerializer.Serialize(userData, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(Environments.GetBackendApiUrl() + BackendEndpoints.SignUp,
            content);

        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("User registered successfully");
            return await response.Content.ReadFromJsonAsync<AuthenticatedUser>();
        }

        var errorResponse =
            await response.Content.ReadFromJsonAsync<BackendErrorResponse<Dictionary<string, List<string>>>>();

        if (errorResponse?.Data != null)
        {
            if (errorResponse.Data.TryGetValue("username", out var usernameErrors) && usernameErrors.Any())
            {
                throw new Exception($"UsernameError: {usernameErrors.First()}");
            }

            if (errorResponse.Data.TryGetValue("email", out var emailErrors) && emailErrors.Any())
            {
                throw new Exception($"EmailError: {emailErrors.First()}");
            }
        }

        string errorDetails =
            JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions { WriteIndented = true });
        throw new Exception($"Failed to register: {errorDetails}");
    }

    public async Task<SignInResponse?> SignInUserAsync(string email, string password)
    {
        var userData = new { email, password };
        var jsonRequest = JsonSerializer.Serialize(userData,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(Environments.GetBackendApiUrl() + BackendEndpoints.SignIn,
            content);

        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("User signed in successfully");
            return await response.Content.ReadFromJsonAsync<SignInResponse>();
        }

        var errorResponse = await response.Content.ReadAsStringAsync();
        _logger.LogError($"Failed to sign in: {errorResponse}");
        throw new Exception($"Failed to sign in: {errorResponse}");
    }

    public async Task<AuthenticatedUser?> GetUserFromToken(string accessToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get,
            $"{Environments.GetBackendApiUrl()}{BackendEndpoints.GetUserFromToken}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        try
        {
            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<AuthenticatedUser>();
            }

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                _logger.LogWarning("Access token is invalid or expired.");
                throw new Exception("Token is invalid or expired. Please log in again.");
            }

            var errorResponse = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed to fetch user from token: {errorResponse}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in GetUserFromToken: {ex.Message}");
            throw;
        }
    }

    public async Task<SignInResponse?> RefreshTokenAsync(string refreshToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Post,
            $"{Environments.GetBackendApiUrl()}{BackendEndpoints.RefreshToken}");
        request.Content = new StringContent(JsonSerializer.Serialize(new { refresh = refreshToken }), Encoding.UTF8,
            "application/json");

        var response = await _httpClient.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<SignInResponse>();
        }

        var errorResponse = await response.Content.ReadAsStringAsync();
        throw new Exception($"Failed to refresh token: {errorResponse}");
    }
}