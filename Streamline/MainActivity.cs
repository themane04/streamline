using AndroidX.RecyclerView.Widget;
using Streamline.Resources.adapter;
using Streamline.Resources.service;

namespace Streamline;

[Activity(Label = "@string/app_name", MainLauncher = true)]
public class MainActivity : Activity
{
    protected override async void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        try
        {
            SetContentView(Resource.Layout.homepage);
            ActionBar?.Hide();
            await LoadMoviesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in OnCreate: {ex.Message}");
            Toast.MakeText(this, $"Error: {ex.Message}", ToastLength.Long)?.Show();
        }
    }

    private async Task LoadMoviesAsync()
    {
        try
        {
            var movies = await MovieService.GetPopularMoviesAsync();
            var recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);

            if (recyclerView != null)
            {
                recyclerView.SetLayoutManager(new LinearLayoutManager(this));
                recyclerView.SetAdapter(new MovieAdapter(this, movies));
            }
            else
            {
                throw new NullReferenceException("RecyclerView not found");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in LoadMoviesAsync: {ex.Message}");
            Toast.MakeText(this, $"Failed to load movies: {ex.Message}", ToastLength.Long)?.Show();
        }
    }
}
