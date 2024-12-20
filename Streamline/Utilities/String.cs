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
}