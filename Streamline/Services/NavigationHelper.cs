using Android.Util;

namespace Streamline.Services;

public static class NavigationHelper
{
    public static async Task NavigateToHomepageAsync(Activity activity)
    {
        try
        {
            activity.SetContentView(Resource.Layout.homepage);
            activity.ActionBar?.Hide();
            await MovieService.LoadMoviesAsync(activity);
        }
        catch (Exception ex)
        {
            Log.Error("NavigationHelper", $"Error navigating to homepage: {ex}");
            Toast.MakeText(activity, $"Error: {ex.Message}", ToastLength.Long)?.Show();
        }
    }
}