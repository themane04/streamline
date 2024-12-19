using Android.Content.Res;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Streamline.Contexts;
using Streamline.Services;

namespace Streamline;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();
        builder.Services.AddMauiBlazorWebView();
        builder.Services.AddSingleton<MovieService>();
        builder.Services.AddDbContext<MovieDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultString")));
    
#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif
    
        return builder.Build();
    }
}