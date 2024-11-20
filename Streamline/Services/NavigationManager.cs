using Streamline.Enums;
using Streamline.Listeners;

namespace Streamline.Services;

public class NavigationManager
{
    private readonly Activity _activity;

    public NavigationManager(Activity activity)
    {
        _activity = activity;
    }

    public void NavigateTo(Screen screen)
    {
        switch (screen)
        {
            case Screen.Home:
                _activity.SetContentView(Resource.Layout.homepage);
                break;
            case Screen.SignIn:
                _activity.SetContentView(Resource.Layout.log_in);
                SignInListeners.Setup(_activity, this);
                break;
            case Screen.SignUp:
                _activity.SetContentView(Resource.Layout.sign_up);
                SignUpListeners.Setup(_activity, this);
                break;
        }
    }
}