using Android.Views;
using AndroidX.RecyclerView.Widget;
using Streamline.Modules.adapter;
using Streamline.Services;

namespace Streamline;

[Activity(Label = "@string/app_name", MainLauncher = true)]
public class MainActivity : Activity
{
    protected override async void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        try
        {
            SetContentView(Resource.Layout.sign_in);
            ActionBar?.Hide();

            SetupSignInListeners();
            await LoadMoviesAsync();
        }
        catch (Exception ex)
        {
            Android.Util.Log.Error("MainActivity", $"Error in OnCreate: {ex}");
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
            Android.Util.Log.Error("MainActivity", $"Error in LoadMoviesAsync: {ex}");
            Toast.MakeText(this, $"Failed to load movies: {ex.Message}", ToastLength.Long)?.Show();
        }
    }

    private void SetupSignInListeners()
    {
        TextView signUpTextView = FindViewById<TextView>(Resource.Id.signup_text);
        signUpTextView?.SetOnClickListener(new ClickListener(this, "signUp"));

        Button loginButton = FindViewById<Button>(Resource.Id.login_button);
        if (loginButton != null)
        {
            loginButton.Click += (sender, args) => NavigateTo(Screen.Home);
        }
    }

    private void NavigateTo(Screen screen)
    {
        switch (screen)
        {
            case Screen.Home:
                SetContentView(Resource.Layout.homepage);
                break;
            case Screen.SignIn:
                SetContentView(Resource.Layout.sign_in);
                SetupSignInListeners();
                break;
            case Screen.SignUp:
                SetContentView(Resource.Layout.sign_up);
                SetupSignUpListeners();
                break;
        }
    }

    private void SetupSignUpListeners()
    {
        TextView signInTextView = FindViewById<TextView>(Resource.Id.signin_text);
        signInTextView?.SetOnClickListener(new ClickListener(this, "signIn"));
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
                _activity.NavigateTo(Screen.SignIn);
            }
            else if (_action == "signUp")
            {
                _activity.NavigateTo(Screen.SignUp);
            }
        }
    }

    private enum Screen
    {
        Home,
        SignIn,
        SignUp
    }
}
