using Android.Views;

namespace Streamline;

[Activity(Label = "@string/app_name", MainLauncher = true)]
public class MainActivity : Activity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        ActionBar?.Hide();
        SetContentView(Resource.Layout.sign_in);
        View rootView = FindViewById(Android.Resource.Id.Content);
        rootView.SetBackgroundColor(Android.Graphics.Color.ParseColor("#FEF3E2"));
    }
}