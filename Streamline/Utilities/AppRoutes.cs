namespace Streamline.Utilities;

public static class AppRoutes
{
    public const string LogInUrl = "/";
    public const  string SignUpUrl = "/signup";
    public const  string HomeUrl = "/home";
    public const  string MovieDetailView = "/movie";
    public const  string WatchlistUrl = "/watchlist";
    public const  string FavoritesUrl = "/favorites";
    public const  string ProfileUrl = "/profile";
    public const  string InfoUrl = "/info";
    public const  string FaqUrl = "/faq";
    public const  string TermsUrl = "/terms-and-conditions";

    public static readonly List<string> ExcludedRoutes = new()
    {
        LogInUrl,
        SignUpUrl,
        TermsUrl
    };
}