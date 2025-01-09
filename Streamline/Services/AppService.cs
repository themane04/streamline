using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Streamline.Models;
using Streamline.Utilities;

namespace Streamline.Services;

public class AppService
{
    private readonly AuthService _authService;
    private readonly UserService _userService;
    private readonly NavigationManager _navigationManager;
    private readonly ILogger<AppService> _logger;
    private Timer? _refreshTokenTimer;

    public bool IsInitialLoading { get; private set; } = true;
    private bool IsInitialized { get; set; }

    public AppService(AuthService authService, UserService userService, NavigationManager navigationManager,
        ILogger<AppService> logger)
    {
        _authService = authService;
        _userService = userService;
        _navigationManager = navigationManager;
        _logger = logger;
        _logger.LogInformation("Initialized");
    }

    public async Task InitializeAsync()
    {
        if (IsInitialized) return;

        IsInitialLoading = true;

        try
        {
            _logger.LogInformation("Initializing AppService...");
            var refreshToken = await SecureStorage.GetAsync("refreshToken");
            var accessToken = await SecureStorage.GetAsync("accessToken");
            var userJson = await SecureStorage.GetAsync("user");

            _logger.LogInformation($"Retrieved Refresh Token: {refreshToken}");
            _logger.LogInformation($"Retrieved Access Token: {accessToken}");
            _logger.LogInformation($"Retrieved User JSON: {userJson}");

            if (string.IsNullOrEmpty(refreshToken) || string.IsNullOrEmpty(accessToken) ||
                string.IsNullOrEmpty(userJson))
            {
                _authService.ClearAuthState();
                _navigationManager.NavigateTo(AppRoutes.LoginUrl);
                return;
            }

            var user = await _userService.GetUserFromToken(accessToken);

            if (user == null)
            {
                _logger.LogWarning("User deserialization failed.");
                _authService.ClearAuthState();
                _navigationManager.NavigateTo(AppRoutes.LoginUrl);
                return;
            }

            _logger.LogInformation($"Restoring user state: {JsonSerializer.Serialize(user)}");

            var signInResponse = new SignInResponse
            {
                Refresh = refreshToken,
                Access = accessToken,
                User = user.Data
            };

            await _authService.SetAuthState(new BackendResponse<SignInResponse>
            {
                Code = 200,
                Message = "User restored from SecureStorage.",
                Endpoint = "/api/auth/signin",
                Data = signInResponse
            });

            _authService.NotifyStateChanged();
            
            ScheduleTokenRefresh();

            if (_navigationManager.ToBaseRelativePath(_navigationManager.Uri).TrimEnd('/') ==
                AppRoutes.LoginUrl.TrimStart('/'))
            {
                _logger.LogInformation("Initialization successful. Redirecting to HomeUrl.");
                _navigationManager.NavigateTo(AppRoutes.HomeUrl);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error during initialization: {ex.Message}");
            _authService.ClearAuthState();
            _navigationManager.NavigateTo(AppRoutes.LoginUrl);
        }
        finally
        {
            IsInitialLoading = false;
            IsInitialized = true;
        }
    }

    private void ScheduleTokenRefresh()
    {
        if (string.IsNullOrEmpty(_authService.RefreshToken) || string.IsNullOrEmpty(_authService.AccessToken))
        {
            _logger.LogWarning("Cannot schedule token refresh. Tokens are missing.");
            return;
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(_authService.AccessToken);
        var expiration = jwtToken.ValidTo;

        var refreshTime = expiration - DateTime.UtcNow - TimeSpan.FromMinutes(1);

        if (refreshTime <= TimeSpan.Zero)
        {
            _logger.LogWarning("Access token is already expired. Attempting to refresh immediately.");
            _ = RefreshTokenAsync();
            return;
        }

        _logger.LogInformation($"Scheduling token refresh in {refreshTime.TotalMinutes.ToString("F0")} minutes.");

        _refreshTokenTimer?.Dispose();
        _refreshTokenTimer = new Timer(async _ => await RefreshTokenAsync(), null, refreshTime, Timeout.InfiniteTimeSpan);
    }

    private async Task RefreshTokenAsync()
    {
        try
        {
            _logger.LogInformation("Refreshing access token...");
            var signInResponse = await _userService.RefreshTokenAsync(_authService.RefreshToken!);

            if (signInResponse != null)
            {
                await _authService.SetAuthState(signInResponse);
                ScheduleTokenRefresh();
                _logger.LogInformation("Token refreshed successfully.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Token refresh failed: {ex.Message}");
            _authService.ClearAuthState();
            _navigationManager.NavigateTo(AppRoutes.LoginUrl);
        }
    }
}