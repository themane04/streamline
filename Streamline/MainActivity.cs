using Streamline.Enums;
using Streamline.Listeners;
using Streamline.Services;
using Streamline.Utilities;

namespace Streamline;

[Activity(Label = "@string/app_name", MainLauncher = true)]
public class MainActivity : Activity
{
    private NavigationManager? _navigationManager;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        try
        {
            SetContentView(Resource.Layout.log_in);
            ActionBar?.Hide();
            _navigationManager = new NavigationManager(this);
            SignInListeners.Setup(this, _navigationManager);
        }
        catch (Exception ex)
        {
            LogHelper.Log(LogLevel.Error, "MainActivity", $"Error: {ex.Message}");
            Toast.MakeText(this, $"Error: {ex.Message}", ToastLength.Long)?.Show();
        }
    }
    
    protected override void OnResume()
    {
        base.OnResume();
        NavigationHelper.UpdateNavigationBarVisibility(this);
    }
}