using Streamline.Models;

namespace Streamline.Services;

public class AuthService
{
    public string? RefreshToken { get; private set; }
    public string? AccessToken { get; private set; }
    public AuthenticatedUser? User { get; private set; }

    public event Action? OnChange;

    public void SetAuthState(SignInResponse signInResponse)
    {
        RefreshToken = signInResponse.Refresh;
        AccessToken = signInResponse.Access;
        User = signInResponse.User;

        NotifyStateChanged();
    }

    public void ClearAuthState()
    {
        RefreshToken = null;
        AccessToken = null;
        User = null;

        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}