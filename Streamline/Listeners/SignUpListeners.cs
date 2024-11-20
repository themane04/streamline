using Streamline.Enums;
using Streamline.Services;

namespace Streamline.Listeners;

public static class SignUpListeners
{
    public static void Setup(Activity activity, NavigationManager navigationManager)
    {
        TextView? signInTextView = activity.FindViewById<TextView>(Resource.Id.signin_text);

        if (signInTextView != null)
        {
            signInTextView.SetOnClickListener(new ClickListener(navigationManager, Screen.SignIn));
        }
        else
        {
            Android.Util.Log.Warn("SignUpListeners", "signIn_text TextView not found in the current layout.");
        }
    }
}