using Android.Content;
using JakeWharton.Picasso;

namespace Streamline.Utilities;

using Square.Picasso;

public class PicassoSingleton
{
    private static Picasso? _instance;

    public static Picasso GetInstance(Context context)
    {
        if (_instance == null)
        {
            _instance = new Picasso.Builder(context)
                .Downloader(new OkHttp3Downloader(context))
                .Build();
        }

        return _instance;
    }
}
