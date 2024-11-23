using Streamline.Enums;
using Streamline.Services;
using Streamline.Utilities;

namespace Streamline.Listeners;

public static class SignUpListener
{
    public static void Setup(Activity activity, NavigationManagerService navigationManagerService)
    {
        TextView? signInTextView = activity.FindViewById<TextView>(Resource.Id.login_text);
        if (signInTextView != null)
        {
            signInTextView.SetOnClickListener(new ClickListener(navigationManagerService, Screen.SignIn));
        }
        else
        {
            LogHelper.Log(LogLevel.Warn, "SignUpListeners", "signin_text TextView not found in the layout.");
        }

        Button? signUpButton = activity.FindViewById<Button>(Resource.Id.signup_button);
        if (signUpButton != null)
        {
            signUpButton.Click += (sender, args) =>
            {
                navigationManagerService.NavigateTo(Screen.SignIn);
                Toast.MakeText(activity, "Sign up successful.", ToastLength.Short)?.Show();
            };
        }
        else
        {
            LogHelper.Log(LogLevel.Warn, "SignUpListeners", "signup_button Button not found in the layout.");
        }
    }
}