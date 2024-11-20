using Streamline.Enums;
using Streamline.Services;

namespace Streamline.Listeners;

public static class SignUpListeners
{
    public static void Setup(Activity activity, NavigationManager navigationManager)
    {
        TextView signInTextView = activity.FindViewById<TextView>(Resource.Id.signin_text);
        signInTextView?.SetOnClickListener(new ClickListener(navigationManager, Screen.SignIn));
    }
}
