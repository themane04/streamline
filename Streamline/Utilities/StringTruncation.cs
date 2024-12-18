namespace Streamline.Utilities;

public class StringTruncation
{
    public static string Truncate(string? input, int maxLength)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;
        return input.Length > maxLength ? input.Substring(0, maxLength) + "..." : input;
    }
}