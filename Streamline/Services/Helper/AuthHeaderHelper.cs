using System.Net.Http.Headers;

namespace Streamline.Services.Helper;

public class AuthHeaderHelper
{
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