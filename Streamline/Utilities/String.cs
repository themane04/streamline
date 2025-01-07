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
}