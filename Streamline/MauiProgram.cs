using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Streamline.Contexts;
using Streamline.Services;
using Streamline.Utilities;

namespace Streamline;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();
        builder.Services.AddMauiBlazorWebView();
        builder.Services.AddDbContext<MovieDbContext>(options =>
            options.UseNpgsql(Environments.GetConnectionString()));
        builder.Services.AddScoped<MovieService>(serviceProvider =>
            new MovieService(serviceProvider.GetRequiredService<MovieDbContext>()));
        
#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}