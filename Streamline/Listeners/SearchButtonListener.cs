using Android.Views;
using Streamline.Adapters;
using Streamline.Enums;
using Streamline.Models;
using Streamline.Utilities;

namespace Streamline.Listeners;

public static class SearchButtonListener
{
    public static void Setup(Activity activity, List<Movie> allMovies, MovieAdapter adapter)
    {
        var searchButton = activity.FindViewById<ImageButton>(Resource.Id.searchButton);
        var searchInput = activity.FindViewById<EditText>(Resource.Id.searchInput);

        if (searchButton == null || searchInput == null)
        {
            LogHelper.Log(LogLevel.Error, "SearchButtonListener", "SearchButton or SearchInput is null.");
            return;
        }

        searchButton.Click += (sender, args) =>
        {
            searchButton.Animate()?
                .ScaleX(0f)
                .ScaleY(0f)
                .Alpha(0f)
                .SetDuration(300)
                .WithEndAction(new Java.Lang.Runnable(() =>
                {
                    searchButton.Visibility = ViewStates.Gone;

                    var layoutParams = searchInput.LayoutParameters;
                    if (layoutParams != null)
                    {
                        layoutParams.Width = ViewGroup.LayoutParams.MatchParent;
                        searchInput.LayoutParameters = layoutParams;
                    }
                    else
                    {
                        LogHelper.Log(LogLevel.Error, "SearchButtonListener", "SearchInput LayoutParameters is null.");
                    }

                    searchInput.Visibility = ViewStates.Visible;
                    searchInput.Animate()?
                        .Alpha(1f)
                        .SetDuration(300)
                        .Start();
                }))
                .Start();
        };

        searchInput.FocusChange += (sender, e) =>
        {
            if (!e.HasFocus)
            {
                searchInput.Animate()?
                    .Alpha(0f)
                    .SetDuration(300)
                    .WithEndAction(new Java.Lang.Runnable(() =>
                    {
                        searchInput.Visibility = ViewStates.Gone;

                        searchButton.Visibility = ViewStates.Visible;
                        searchButton.Animate()?
                            .ScaleX(1f)
                            .ScaleY(1f)
                            .Alpha(1f)
                            .SetDuration(300)
                            .Start();
                    }))
                    .Start();
            }
        };

        searchInput.TextChanged += (sender, args) =>
        {
            var query = args.Text?.ToString()?.Trim().ToLower();
            LogHelper.Log(LogLevel.Info, "SearchButtonListener", $"Query: {query}");

            if (!string.IsNullOrEmpty(query))
            {
                var filteredMovies = allMovies
                    .Where(movie => !string.IsNullOrEmpty(movie.Title) && movie.Title.ToLower().Contains(query))
                    .ToList();

                LogHelper.Log(LogLevel.Info, "SearchButtonListener", $"Filtered movies count: {filteredMovies.Count}");
                adapter.UpdateMovies(filteredMovies);
            }
            else
            {
                adapter.UpdateMovies(allMovies);
            }
        };
    }
}