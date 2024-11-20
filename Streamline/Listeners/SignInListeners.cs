using Android.Util;
using Streamline.Enums;
using Streamline.Services;
using Streamline.Utilities;

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
            LogHelper.Log(LogLevel.Warn, "SignInListeners", "signup_text TextView not found in the layout.");
        }

        Button? loginButton = activity.FindViewById<Button>(Resource.Id.login_button);
        if (loginButton != null)
        {
            loginButton.Click += async (sender, args) =>
            {
                LogHelper.Log(LogLevel.Info, "SignInListeners", "login_button Button clicked.");
                await NavigationHelper.NavigateToHomepageAsync(activity);
                Toast.MakeText(activity, "Login successful.", ToastLength.Short)?.Show();
            };
        }
        else
        {
            LogHelper.Log(LogLevel.Warn, "SignInListeners", "login_button Button not found in the layout.");
        }
    }
}