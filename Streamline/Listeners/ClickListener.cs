using Android.Views;
using Streamline.Enums;
using Streamline.Services;

namespace Streamline.Listeners;

public class ClickListener : Java.Lang.Object, View.IOnClickListener
{
    private readonly NavigationManager _navigationManager;
    private readonly Screen _action;

    public ClickListener(NavigationManager navigationManager, Screen action)
    {
        _navigationManager = navigationManager;
        _action = action;
    }

    public void OnClick(View? v)
    {
        _navigationManager.NavigateTo(_action);
    }
}
