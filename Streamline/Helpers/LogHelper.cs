using Streamline.Enums;

namespace Streamline.Utilities;

public static class LogHelper
{
    private static readonly string AppTag = "StreamlineApp";
    private static readonly LogLevel MinimumLogLevel = LogLevel.Info;

    public static void Log(LogLevel level, string tag, string message)
    {
        if (level >= MinimumLogLevel)
        {
            string formattedMessage = $"[{tag}] {message}";

            switch (level)
            {
                case LogLevel.Verbose:
                    Android.Util.Log.Verbose(AppTag, formattedMessage);
                    break;
                case LogLevel.Debug:
                    Android.Util.Log.Debug(AppTag, formattedMessage);
                    break;
                case LogLevel.Info:
                    Android.Util.Log.Info(AppTag, formattedMessage);
                    break;
                case LogLevel.Warn:
                    Android.Util.Log.Warn(AppTag, formattedMessage);
                    break;
                case LogLevel.Error:
                    Android.Util.Log.Error(AppTag, formattedMessage);
                    break;
            }
        }
    }
}
