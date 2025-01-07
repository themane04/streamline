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

    public bool IsInitialLoading { get; private set; } = true;
    private bool IsInitialized { get; set; }

    public AppService(AuthService authService, UserService userService, NavigationManager navigationManager,
        ILogger<AppService> logger)
    {
        _authService = authService;
        _userService = userService;
        _navigationManager = navigationManager;
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        if (IsInitialized) return;

        IsInitialLoading = true;

        try
        {
            var refreshToken = await SecureStorage.GetAsync("refreshToken");
            var accessToken = await SecureStorage.GetAsync("accessToken");

            if (string.IsNullOrEmpty(refreshToken) || string.IsNullOrEmpty(accessToken))
            {
                _authService.ClearAuthState();
                _navigationManager.NavigateTo(AppRoutes.LoginUrl);
                return;
            }

            var user = await _userService.GetUserFromToken(accessToken);

            if (user == null)
            {
                _authService.ClearAuthState();
                _navigationManager.NavigateTo(AppRoutes.LoginUrl);
                return;
            }

            _authService.SetAuthState(new SignInResponse
            {
                Refresh = refreshToken,
                Access = accessToken,
                User = user
            });

            if (_navigationManager.Uri == _navigationManager.BaseUri.TrimEnd('/'))
            {
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

}