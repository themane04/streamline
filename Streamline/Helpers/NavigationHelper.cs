using Android.Views;
using AndroidX.RecyclerView.Widget;
using Streamline.Adapters;
using Streamline.Enums;
using Streamline.Listeners;
using Streamline.Models;
using Streamline.Utilities;

namespace Streamline.Services;

public static class NavigationHelper
{
    public static async Task NavigateToHomepageAsync(Activity activity)
    {
        try
        {
            activity.SetContentView(Resource.Layout.base_layout);

            var contentFrame = activity.FindViewById<FrameLayout>(Resource.Id.contentFrame);
            if (contentFrame != null)
            {
                LayoutInflater.From(activity).Inflate(Resource.Layout.homepage, contentFrame, true);

                LogHelper.Log(LogLevel.Info, "NavigationHelper", "Navigated to homepage.");

                var recyclerView = activity.FindViewById<RecyclerView>(Resource.Id.recyclerView);
                if (recyclerView != null)
                {
                    int spanCount = ResponsiveGrid.GetSpanCount(activity);

                    var layoutManager = new GridLayoutManager(activity, spanCount);
                    recyclerView.SetLayoutManager(layoutManager);

                    var adapter = new MovieAdapter(activity, new List<Movie>());
                    recyclerView.SetAdapter(adapter);

                    SearchButtonListener.Setup(activity, MovieService.allMovies, adapter);

                    await MovieService.LoadMoviesAsync(activity, 1, adapter);

                    recyclerView.AddOnScrollListener(new EndlessScrollListener(layoutManager, page =>
                        MovieService.LoadMoviesAsync(activity, page, adapter)));
                }
                else
                {
                    LogHelper.Log(LogLevel.Warn, "NavigationHelper", "RecyclerView not found in the layout.");
                    Toast.MakeText(activity, "Failed to load movies: RecyclerView not found.", ToastLength.Long)
                        ?.Show();
                }
            }
            else
            {
                LogHelper.Log(LogLevel.Error, "NavigationHelper", "Content frame not found in base layout.");
                Toast.MakeText(activity, "Content frame not found.", ToastLength.Long)?.Show();
            }

            UpdateNavigationBarVisibility(activity);
        }
        catch (Exception ex)
        {
            LogHelper.Log(LogLevel.Error, "NavigationHelper", $"Error: {ex.Message}");
            Toast.MakeText(activity, $"Error: {ex.Message}", ToastLength.Long)?.Show();
        }
    }

    public static void UpdateNavigationBarVisibility(Activity activity)
    {
        var navigationBar = activity.FindViewById<LinearLayout>(Resource.Id.navigationBar);

        if (navigationBar == null)
        {
            LogHelper.Log(LogLevel.Warn, "NavigationHelper", "Navigation bar view not found.");
            return;
        }

        bool isSignInLayout = activity.FindViewById(Resource.Id.login_page) != null;
        bool isSignUpLayout = activity.FindViewById(Resource.Id.signup_page) != null;

        navigationBar.Visibility = (isSignInLayout || isSignUpLayout) ? ViewStates.Gone : ViewStates.Visible;
    }
}