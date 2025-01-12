using Streamline.Models;

namespace Streamline.Utilities;

public class Statics
{
    public static string Truncate(string? input, int maxLength)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;
        return input.Length > maxLength ? input.Substring(0, maxLength) + "..." : input;
    }

    public static bool ImageStringAvailable(string? input)
    {
        return !string.IsNullOrEmpty(input) && input.Length > "https://image.tmdb.org/t/p/w500".Length;
    }

    public static string GetSignUpInputFieldClass(bool hasError) =>
        "sign-up-input-field scale-on-interaction" + (hasError ? " sign-up-field-error-border" : "");

    public static string GetSignInInputFieldClass(bool hasError) =>
        "sign-in-input-field scale-on-interaction" + (hasError ? " sign-in-field-error-border" : "");

    public static string GetProfilePasswordResetInputFieldClass(bool hasError) =>
        "profile-page-input-field scale-on-interaction" + (hasError ? " profile-page-field-error-border" : "");

    public static string ModalClass(bool isVisible) => isVisible ? "modal-show" : "modal-hide";
    public static string ModalStyle(string width, string height) => $"width: {width}; height: {height};";

    public static string DrawerStyle(bool isDrawerOpen) => isDrawerOpen ? "right: 0;" : "right: -400px;";

    public static string AuthButtonClass(bool isLoading) =>
        "mud-auth-button scale-on-interaction" + (isLoading ? " mud-auth-button-disabled" : "");

    public static string SearchButtonClass(bool isSearchActive) => isSearchActive
        ? "mud-search-button-container-hp expanded"
        : "mud-search-button-container-hp scale-on-interaction";

    public static string DetailPageButtonClass(bool isMovieIn) =>
        "mud-auth-button mud-movie-detail-button scale-on-interaction"
        + (isMovieIn ? " mud-auth-button-disabled" : "");

    public static List<FAQItem> FaQs = new()
    {
        new FAQItem
        {
            Id = 1,
            Question = "How can I add movies to my Watchlist?",
            Answer =
                "On the movie detail page, click the 'Add to Watchlist' button. The movie will be saved to your watchlist, which you can access anytime from the Watchlist page."
        },
        new FAQItem
        {
            Id = 2,
            Question = "What is the Homepage (Showcase) feature?",
            Answer =
                "The Homepage showcases movies fetched using TMDB's API with infinite scrolling. Clicking on a movie provides detailed information, including the title, genres, release date, popularity, and more."
        },
        new FAQItem
        {
            Id = 3,
            Question = "What details can I see on the Movie Detail page?",
            Answer =
                "The Movie Detail page displays an overview of the movie, including its title, tagline, genres, runtime, release date, popularity, and vote average. You'll also see the movie poster, backdrop image, and links to additional resources like the homepage or IMDb."
        },
        new FAQItem
        {
            Id = 4,
            Question = "How do I manage my Watchlist?",
            Answer =
                "You can view your watchlist on the Watchlist page. To remove a movie, click the 'X' button. To view more details about a movie, click on it to navigate to the Movie Detail page."
        },
        new FAQItem
        {
            Id = 5,
            Question = "How do I manage my Favorites?",
            Answer =
                "Favorites are managed similarly to the Watchlist. Go to the Favorites page to see your saved movies. Click on a movie for details or the 'X' button to remove it from the list."
        },
        new FAQItem
        {
            Id = 6,
            Question = "What can I do on the Profile page?",
            Answer =
                "The Profile page lets you customize your profile by uploading a profile image, editing your username and email, resetting your password, accessing the FAQ page, signing out, or deleting your account."
        },
        new FAQItem
        {
            Id = 7,
            Question = "Can I see detailed statistics about movies?",
            Answer =
                "Yes, detailed statistics such as vote count, average rating, runtime, and revenue are available on the Movie Detail page for movies fetched using the TMDB API."
        },
        new FAQItem
        {
            Id = 8,
            Question = "How does the infinite scroll work on the Homepage?",
            Answer =
                "The infinite scroll feature on the Homepage dynamically loads additional movies as you scroll down, ensuring a seamless browsing experience without needing to click 'Next'."
        },
        new FAQItem
        {
            Id = 9,
            Question = "Can I delete my account from the Profile page?",
            Answer =
                "Yes, the Profile page includes an option to delete your account. Be cautious as this action is irreversible and will remove all your data from the platform."
        }
    };
    
    public static readonly string SignUpDataStorageKey = "SignUpData";
    public static string FormattedDate(string date) => DateTime.Parse(date).ToString("dd.MM.yyyy");
    public static string BackendBirthdayFormat(DateTime date) => date.ToString("dd-MM-yyyy");
}