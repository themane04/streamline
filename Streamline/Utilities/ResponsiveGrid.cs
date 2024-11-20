namespace Streamline.Utilities;

public class ResponsiveGrid
{
    public static int GetSpanCount(Activity activity)
    {
        var displayMetrics = activity.Resources.DisplayMetrics;
        var screenWidthDp = (int)(displayMetrics.WidthPixels / displayMetrics.Density);

        return screenWidthDp switch
        {
            >= 1280 => 5,
            >= 960 => 4,
            >= 600 => 3,
            >= 360 => 2,
            _ => 1
        };
    }
}