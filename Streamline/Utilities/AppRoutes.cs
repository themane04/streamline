namespace Streamline.Utilities;

public static class AppRoutes
{
    public const string LoginUrl = "/";
    public const  string SignupUrl = "/signup";
    public const  string HomeUrl = "/home";
    public const  string MovieDetailView = "/movie";
    public const  string BookmarkUrl = "/bookmark";
    public const  string FavoritesUrl = "/favorites";
    public const  string ProfileUrl = "/profile";

    public static readonly List<string> ExcludedRoutes = new()
    {
        LoginUrl,
        SignupUrl
    };
}