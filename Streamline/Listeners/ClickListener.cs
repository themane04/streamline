using Android.Views;
using Streamline.Enums;
using Streamline.Services;

namespace Streamline.Listeners;

public class ClickListener : Java.Lang.Object, View.IOnClickListener
{
    private readonly NavigationManagerService _navigationManagerService;
    private readonly Screen _action;

    public ClickListener(NavigationManagerService navigationManagerService, Screen action)
    {
        _navigationManagerService = navigationManagerService;
        _action = action;
    }

    public void OnClick(View? v)
    {
        _navigationManagerService.NavigateTo(_action);
    }
}
