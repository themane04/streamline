using Streamline.Listeners;
using Streamline.Services;

namespace Streamline;

[Activity(Label = "@string/app_name", MainLauncher = true)]
public class MainActivity : Activity
{
    private NavigationManager _navigationManager;

    protected override async void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        try
        {
            SetContentView(Resource.Layout.sign_in);
            ActionBar?.Hide();

            _navigationManager = new NavigationManager(this);
            SignInListeners.Setup(this, _navigationManager);

            await MovieService.LoadMoviesAsync(this);
        }
        catch (Exception ex)
        {
            Android.Util.Log.Error("MainActivity", $"Error in OnCreate: {ex}");
            Toast.MakeText(this, $"Error: {ex.Message}", ToastLength.Long)?.Show();
        }
    }
}