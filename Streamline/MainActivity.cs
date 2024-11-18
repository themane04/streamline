using Android.Views;

namespace Streamline;

[Activity(Label = "@string/app_name", MainLauncher = true)]
public class MainActivity : Activity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        ActionBar?.Hide();
        GoToSignIn();
    }

    private void GoToSignIn()
    {
        SetContentView(Resource.Layout.sign_in);
        TextView signUpTextView = FindViewById<TextView>(Resource.Id.signup_text);
        signUpTextView?.SetOnClickListener(new ClickListener(this, "signUp"));
    }

    private void GoToSignUp()
    {
        SetContentView(Resource.Layout.sign_up);
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
                _activity.GoToSignIn();
            }
            else if (_action == "signUp")
            {
                _activity.GoToSignUp();
            }
        }
    }
}