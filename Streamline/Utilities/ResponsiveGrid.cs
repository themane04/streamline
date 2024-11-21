using Streamline.Enums;

namespace Streamline.Utilities;

public class ResponsiveGrid
{
    public static int GetSpanCount(Activity activity)
    {
        if (activity?.Resources?.DisplayMetrics == null)
        {
            LogHelper.Log(LogLevel.Error, "ResponsiveGrid", "Activity or DisplayMetrics is null.");
            return 1;
        }

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