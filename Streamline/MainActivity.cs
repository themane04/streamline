using Android.Views;
using AndroidX.RecyclerView.Widget;
using Streamline.Modules.adapter;
using Streamline.Modules.service;

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
            GoToSignIn();
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

    private void GoToSignUp()
    {
        SetContentView(Resource.Layout.sign_up);
        TextView signInTextView = FindViewById<TextView>(Resource.Id.signin_text);
        signInTextView?.SetOnClickListener(new ClickListener(this, "signIn"));
    }

    private void GoToSignIn()
    {
        SetContentView(Resource.Layout.sign_in);
        TextView signUpTextView = FindViewById<TextView>(Resource.Id.signup_text);
        signUpTextView?.SetOnClickListener(new ClickListener(this, "signUp"));
    }

    private class ClickListener : Java.Lang.Object, View.IOnClickListener
    {
        private readonly MainActivity _activity;
        private readonly string _action;

        public ClickListener(MainActivity activity, string action)
        {
            _activity = activity;
            _action = action;
        }

        public void OnClick(View? v)
        {
            if (_action == "signIn")
            {
                _activity.GoToSignIn();
            }
            else if (_action == "signUp")
            {
                _activity.GoToSignUp();
            }
        }
    }
}