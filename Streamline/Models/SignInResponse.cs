namespace Streamline.Models;

public class SignInResponse
{
    public required string Refresh { get; set; }
    public required string Access { get; set; }
    public required AuthenticatedUser User { get; set; }
}