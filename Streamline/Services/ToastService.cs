using Streamline.Models.Enums;

namespace Streamline.Services;

public class ToastService
{
    public event Action<string, ToastStatus>? OnShowToast;
    
    public void ShowToast(string message, ToastStatus status)
    {
        OnShowToast?.Invoke(message, status);
    }
}