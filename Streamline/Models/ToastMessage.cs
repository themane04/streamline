namespace Streamline.Models;

public class ToastMessage
{
    public required string Message { get; set; }
    public required string Status { get; set; }
    public bool IsVisible { get; set; }
}