using Streamline.Enums;
using Streamline.Services;

namespace Streamline.Listeners;

public static class SignInListeners
{
    public static void Setup(Activity activity, NavigationManager navigationManager)
    {
        TextView? signUpTextView = activity.FindViewById<TextView>(Resource.Id.signup_text);
        if (signUpTextView != null)
        {
            signUpTextView.SetOnClickListener(new ClickListener(navigationManager, Screen.SignUp));
        }
        else
        {
            Android.Util.Log.Warn("SignInListeners", "signup_text TextView not found in the layout.");
        }

        Button? loginButton = activity.FindViewById<Button>(Resource.Id.login_button);
        if (loginButton != null)
        {
            loginButton.Click += (sender, args) => navigationManager.NavigateTo(Screen.Home);
        }
        else
        {
            Android.Util.Log.Warn("SignInListeners", "login_button Button not found in the layout.");
        }
    }
}
