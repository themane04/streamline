using Streamline.Enums;
using Streamline.Services;

namespace Streamline.Listeners;

public static class SignInListeners
{
    public static void Setup(Activity activity, NavigationManager navigationManager)
    {
        TextView signUpTextView = activity.FindViewById<TextView>(Resource.Id.signup_text);
        signUpTextView?.SetOnClickListener(new ClickListener(navigationManager, Screen.SignUp));

        Button loginButton = activity.FindViewById<Button>(Resource.Id.login_button);
        if (loginButton != null)
        {
            loginButton.Click += (sender, args) => navigationManager.NavigateTo(Screen.Home);
        }
    }
}
