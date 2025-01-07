using System.Text.RegularExpressions;

namespace Streamline.Utilities;

public class Validation
{
    private static readonly Regex UsernameRegex = new("^[a-zA-Z0-9_-]{3,16}$");
    private static readonly Regex EmailRegex = new(@"^(([^<>()\]\\.,;:\s@""]+(\.[^<>()\]\\.,;:\s@""]+)*)|("".+""))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$");
    private static readonly Regex PasswordRegex = new(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&*()_+\-=[\]{};':""\\|,.<>/?])(?!.*\s).{8,255}$");
    
    public static bool IsUsernameValid(string username)
    {
        return UsernameRegex.IsMatch(username);
    }
    
    public static bool IsEmailValid(string email)
    {
        return EmailRegex.IsMatch(email);
    }
    
    public static bool IsPasswordValid(string password)
    {
        return PasswordRegex.IsMatch(password);
    }
    
    public static bool IsPasswordMatch(string password, string confirmPassword)
    {
        return password == confirmPassword;
    }
}