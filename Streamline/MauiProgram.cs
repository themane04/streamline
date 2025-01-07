using Microsoft.Extensions.Logging;
using MudBlazor;
using MudBlazor.Services;
using Streamline.Services;
using Streamline.Services.Helper;

namespace Streamline;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();
        builder.Services.AddMauiBlazorWebView();
        builder.Services.AddSingleton<HttpClient>();
        builder.Services.AddSingleton<MovieServiceHelper>();
        builder.Services.AddSingleton<MovieService>();
        builder.Services.AddSingleton<UserService>();
        builder.Services.AddSingleton<ToastService>();
        builder.Services.AddScoped<AuthService>();
        builder.Services.AddScoped<AppService>();

        ConfigureLogging(builder.Logging);
#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif
        return builder.Build();
    }

    private static void ConfigureLogging(ILoggingBuilder logging)
    {
        logging.ClearProviders();
        logging.AddDebug();
        logging.AddConsole();
    }
}