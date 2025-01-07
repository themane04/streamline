using System.Text.Json;
using Microsoft.Extensions.Logging;
using Streamline.Models;

namespace Streamline.Services;

public class AuthService
{
    public string? RefreshToken { get; private set; }
    public string? AccessToken { get; private set; }
    public AuthenticatedUser? User { get; private set; }
    private readonly ILogger<AuthService> _logger;

    public AuthService(ILogger<AuthService> logger)
    {
        _logger = logger;
    }

    public event Action? OnChange;

    public async Task SetAuthState(SignInResponse signInResponse)
    {
        RefreshToken = signInResponse.Refresh;
        AccessToken = signInResponse.Access;
        User = signInResponse.User;

        await SecureStorage.SetAsync("refreshToken", RefreshToken);
        await SecureStorage.SetAsync("accessToken", AccessToken);
        await SecureStorage.SetAsync("user", JsonSerializer.Serialize(User));

        _logger.LogInformation($"Saved Refresh Token: {RefreshToken}");
        _logger.LogInformation($"Saved Access Token: {AccessToken}");
        _logger.LogInformation($"Saved User: {JsonSerializer.Serialize(User)}");

        NotifyStateChanged();
    }

    public void ClearAuthState()
    {
        RefreshToken = null;
        AccessToken = null;
        User = null;

        SecureStorage.Remove("refreshToken");
        SecureStorage.Remove("accessToken");
        SecureStorage.Remove("user");
        
        _logger.LogInformation("Cleared tokens and user data from SecureStorage.");

        NotifyStateChanged();
    }

    public void NotifyStateChanged() => OnChange?.Invoke();
}