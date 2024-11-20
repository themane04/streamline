using System.Text.Json;
using Android.Util;
using AndroidX.RecyclerView.Widget;
using Streamline.Adapters;
using Streamline.Contexts;
using Streamline.Enums;
using Streamline.Models;
using Streamline.Utilities;

namespace Streamline.Services;

public class MovieService
{
    private static readonly string ApiKey = "d9cb9af8ce13f4a3a6a3d19dde83783f";
    private static readonly string BaseUrl = "https://api.themoviedb.org/3/";

    public static async Task<List<Movie>> GetPopularMoviesAsync()
    {
        using HttpClient client = new();
        string url = $"{BaseUrl}movie/popular?api_key={ApiKey}&language=en-US&page=1";

        try
        {
            HttpResponseMessage response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                LogHelper.Log(LogLevel.Info, "MovieService", $"Fetched {json.Length} bytes of movie data.");
                var result = JsonSerializer.Deserialize(json, MovieJsonContext.Default.MovieResponse);
                return result?.Results ?? new List<Movie>();
            }
        }
        catch (Exception ex)
        {
            LogHelper.Log(LogLevel.Error, "MovieService", $"Error fetching movies: {ex.Message}");
        }

        return new List<Movie>();
    }

    public static async Task LoadMoviesAsync(Activity activity)
    {
        try
        {
            var movies = await GetPopularMoviesAsync();
            var recyclerView = activity.FindViewById<RecyclerView>(Resource.Id.recyclerView);

            if (recyclerView != null)
            {
                recyclerView.SetLayoutManager(new LinearLayoutManager(activity));
                var adapter = new MovieAdapter(activity, movies);
                recyclerView.SetAdapter(adapter);

                LogHelper.Log(LogLevel.Info, "MovieService", $"Attached adapter with {movies.Count} movies.");
            }
            else
            {
                LogHelper.Log(LogLevel.Warn, "MovieService", "recyclerView not found in the layout.");
            }
        }
        catch (Exception ex)
        {
            LogHelper.Log(LogLevel.Error, "MovieService", $"Failed to load movies: {ex.Message}");
            Toast.MakeText(activity, $"Failed to load movies: {ex.Message}", ToastLength.Long)?.Show();
        }
    }
}