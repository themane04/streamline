using Android.Util;
using Streamline.Enums;
using Streamline.Utilities;

namespace Streamline.Services;

public static class NavigationHelper
{
    public static async Task NavigateToHomepageAsync(Activity activity)
    {
        try
        {
            activity.SetContentView(Resource.Layout.homepage);

            LogHelper.Log(LogLevel.Info, "NavigationHelper", "Navigated to homepage.");

            await MovieService.LoadMoviesAsync(activity);
        }
        catch (Exception ex)
        {
            LogHelper.Log(LogLevel.Error, "NavigationHelper", $"Error: {ex.Message}");
            Toast.MakeText(activity, $"Error: {ex.Message}", ToastLength.Long)?.Show();
        }
    }
}
