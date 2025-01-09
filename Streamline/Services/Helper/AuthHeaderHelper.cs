using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;

namespace Streamline.Services.Helper;

public class AuthHeaderHelper
{
    private readonly ILogger<AuthHeaderHelper> _logger;

    public AuthHeaderHelper(ILogger<AuthHeaderHelper> logger)
    {
        _logger = logger;
        _logger.LogInformation("Initialized");
    }

    public static void SetAuthorizationHeader(HttpClient httpClient, string? accessToken)
    {
        if (!string.IsNullOrEmpty(accessToken))
        {
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);
        }
        else
        {
            httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }
}