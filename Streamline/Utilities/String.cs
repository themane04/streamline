namespace Streamline.Utilities;

public class String
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

    public static string SaveButtonClass(bool isSaveDisabled) =>
        "mud-auth-button mud-movie-detail-button scale-on-interaction" +
        (isSaveDisabled ? " mud-auth-button-disabled" : "");

    public static string ModalClass(bool isVisible) => isVisible ? "modal-show" : "modal-hide";
    public static string ModalStyle(string width, string height) => $"width: {width}; height: {height};";
    
    public static string DrawerStyle(bool isDrawerOpen) => isDrawerOpen ? "right: 0;" : "right: -300px;";
}