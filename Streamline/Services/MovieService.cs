using System.Text.Json;
using AndroidX.RecyclerView.Widget;
using Streamline.Adapters;
using Streamline.Contexts;
using Streamline.Models;

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
                Android.Util.Log.Info("MovieService", $"Fetched movies: {json}");
                var result = JsonSerializer.Deserialize(json, MovieJsonContext.Default.MovieResponse);
                return result?.Results ?? new List<Movie>();
            }
        }
        catch (Exception ex)
        {
            Android.Util.Log.Error("MovieService", $"Error fetching movies: {ex.Message}");
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
                recyclerView.SetAdapter(new MovieAdapter(activity, movies));
            }
            else
            {
                throw new NullReferenceException("RecyclerView not found in the layout");
            }
        }
        catch (Exception ex)
        {
            Android.Util.Log.Error("MovieService", $"Error loading movies into RecyclerView: {ex}");
            Toast.MakeText(activity, $"Failed to load movies: {ex.Message}", ToastLength.Long)?.Show();
        }
    }
}