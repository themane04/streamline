using Microsoft.Extensions.Logging;
using Streamline.Services;

namespace Streamline;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();
        builder.Services.AddMauiBlazorWebView();
        builder.Services.AddSingleton<HttpClient>();
        builder.Services.AddSingleton<MovieService>();
    
#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif
    
        return builder.Build();
    }
}