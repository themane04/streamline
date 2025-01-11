using Microsoft.Extensions.Logging;
using Streamline.Models;

namespace Streamline.Services;

public class HomepageService
{
    private readonly ILogger<HomepageService> _logger;
    public List<Movie>? Movies { get; set; }
    public int CurrentPage { get; set; } = 1;

    public HomepageService(ILogger<HomepageService> logger)
    {
        _logger = logger;
        _logger.LogInformation("Initialized");
    }

    public void ResetHomepageMovieState()
    {
        Movies = null;
        CurrentPage = 1;
        _logger.LogInformation("Homepage movie state has been reset.");
    }
}