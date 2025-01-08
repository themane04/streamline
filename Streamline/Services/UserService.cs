using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;
using Streamline.Models;
using Streamline.Services.Helper;
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

    public async Task<BackendResponse<AuthenticatedUser>?> SignUpUserAsync(string username, string email,
        string password,
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
            return await response.Content.ReadFromJsonAsync<BackendResponse<AuthenticatedUser>>();
        }

        var errorResponse =
            await response.Content.ReadFromJsonAsync<BackendResponse<Dictionary<string, List<string>>>>();

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

    public async Task<BackendResponse<SignInResponse>?> SignInUserAsync(string email, string password)
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
            return await response.Content.ReadFromJsonAsync<BackendResponse<SignInResponse>>();
        }

        var errorResponse = await response.Content.ReadAsStringAsync();
        _logger.LogError($"Failed to sign in: {errorResponse}");
        throw new Exception($"Failed to sign in: {errorResponse}");
    }

    public async Task<BackendResponse<AuthenticatedUser>?> GetUserFromToken(string accessToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get,
            $"{Environments.GetBackendApiUrl()}{BackendEndpoints.GetUserFromToken}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        try
        {
            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<BackendResponse<AuthenticatedUser>>();
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

    public async Task<BackendResponse<SignInResponse>?> RefreshTokenAsync(string refreshToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Post,
            $"{Environments.GetBackendApiUrl()}{BackendEndpoints.RefreshToken}");
        request.Content = new StringContent(JsonSerializer.Serialize(new { refresh = refreshToken }), Encoding.UTF8,
            "application/json");

        var response = await _httpClient.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<BackendResponse<SignInResponse>>();
        }

        var errorResponse = await response.Content.ReadAsStringAsync();
        throw new Exception($"Failed to refresh token: {errorResponse}");
    }

    public async Task<BackendResponse<AuthenticatedUser>?> UpdateProfilePictureAsync(int? id, IBrowserFile file)
    {
        var accessToken = await SecureStorage.GetAsync("accessToken");
        AuthHeaderHelper.SetAuthorizationHeader(_httpClient, accessToken);

        var content = new MultipartFormDataContent();
        try
        {
            Console.WriteLine("Opening file stream...");
            var stream = file.OpenReadStream(maxAllowedSize: 50 * 1024 * 1024);
            Console.WriteLine($"Stream Length: {stream.Length}, Can Read: {stream.CanRead}");

            var streamContent = new StreamContent(stream);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
            content.Add(streamContent, "profile_image", file.Name);

            content.Headers.ContentLength = stream.Length;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while preparing stream content: {ex.Message}");
            throw;
        }

        var request = new HttpRequestMessage(HttpMethod.Put,
            $"{Environments.GetBackendApiUrl()}{BackendEndpoints.UpdateProfile}/{id}")
        {
            Content = content
        };

        try
        {
            Console.WriteLine("Sending request...");
            var response = await _httpClient.SendAsync(request);
            Console.WriteLine($"Response Status: {response.StatusCode}");

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Response Content:");
                Console.WriteLine(jsonResponse);

                var result = JsonSerializer.Deserialize<BackendResponse<AuthenticatedUser>>(jsonResponse,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return result;
            }

            throw new Exception($"Failed to update profile picture: {await response.Content.ReadAsStringAsync()}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while sending request: {ex.Message}");
            throw;
        }
    }

    public async Task<BackendResponse<AuthenticatedUser>?> UpdateProfileAsync(int? id, string username, string email)
    {
        var accessToken = await SecureStorage.GetAsync("accessToken");
        AuthHeaderHelper.SetAuthorizationHeader(_httpClient, accessToken);

        _logger.LogInformation($"Access Token: {accessToken}");

        var userData = new
        {
            username,
            email
        };

        var jsonRequest = JsonSerializer.Serialize(userData, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

        _logger.LogInformation($"Content for update profile: {jsonRequest}");

        var request = new HttpRequestMessage(HttpMethod.Put,
            $"{Environments.GetBackendApiUrl()}{BackendEndpoints.UpdateProfile}/{id}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        request.Content = content;

        var response = await _httpClient.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<BackendResponse<AuthenticatedUser>>();
            if (result == null)
            {
                throw new Exception("Failed to parse API response.");
            }

            return result;
        }

        var errorResponse = await response.Content.ReadAsStringAsync();
        throw new Exception($"Failed to update user: {errorResponse}");
    }

    public async Task<BackendResponse<JsonElement>> ResetPasswordAsync(int? id, string oldPassword, string newPassword, string confirmPassword)
    {
        var accessToken = await SecureStorage.GetAsync("accessToken");
        AuthHeaderHelper.SetAuthorizationHeader(_httpClient, accessToken);

        var userData = new
        {
            old_password = oldPassword,
            new_password = newPassword,
            confirm_password = confirmPassword
        };

        var jsonRequest = JsonSerializer.Serialize(userData, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

        var request = new HttpRequestMessage(HttpMethod.Put,
            $"{Environments.GetBackendApiUrl()}{BackendEndpoints.ResetPassword}/{id}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        request.Content = content;

        var response = await _httpClient.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<BackendResponse<JsonElement>>();
            if (result == null)
            {
                throw new Exception("Failed to parse API response.");
            }

            return result;
        }

        var errorResponse = await response.Content.ReadFromJsonAsync<BackendResponse<JsonElement>>();
        if (errorResponse?.Data.TryGetProperty("old_password", out var oldPasswordError) == true &&
            oldPasswordError.ValueKind == JsonValueKind.Array &&
            oldPasswordError.GetArrayLength() > 0)
        {
            throw new Exception($"PasswordError: {oldPasswordError[0].GetString()}");
        }

        throw new Exception($"Failed to change password: {JsonSerializer.Serialize(errorResponse)}");
    }
}