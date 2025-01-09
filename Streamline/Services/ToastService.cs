using Microsoft.Extensions.Logging;
using Streamline.Models.Enums;

namespace Streamline.Services;

public class ToastService
{
    public event Action<string, ToastStatus>? OnShowToast;
    private readonly ILogger<ToastService> _logger;

    public ToastService(ILogger<ToastService> logger)
    {
        _logger = logger;
        _logger.LogInformation("Initialized");
    }

    public void ShowToast(string message, ToastStatus status)
    {
        OnShowToast?.Invoke(message, status);
    }
}