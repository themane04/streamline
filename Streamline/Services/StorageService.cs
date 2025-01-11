using System.Text.Json;
using Microsoft.Extensions.Logging;
using String = Streamline.Utilities.Statics;

namespace Streamline.Services;

public class StorageService
{
    private readonly ILogger<AuthService> _logger;


    public StorageService(ILogger<AuthService> logger)
    {
        _logger = logger;
        _logger.LogInformation("Initialized");
    }

    public async Task SaveSignUpDataAsync(Dictionary<string, string> formData)
    {
        try
        {
            var serializedData = JsonSerializer.Serialize(formData);
            await SecureStorage.SetAsync(String.SignUpDataStorageKey, serializedData);
            _logger.LogInformation($"Form data saved to SecureStorage: {serializedData}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error saving data to SecureStorage: {ex.Message}");
        }
    }

    public async Task<Dictionary<string, string>> RetrieveSignUpDataAsync()
    {
        try
        {
            var storedData = await SecureStorage.GetAsync(String.SignUpDataStorageKey);
            if (!string.IsNullOrWhiteSpace(storedData))
            {
                var formData = JsonSerializer.Deserialize<Dictionary<string, string>>(storedData);
                _logger.LogInformation($"Form data retrieved from SecureStorage: {storedData}");
                return formData ?? new Dictionary<string, string>();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving form data: {ex.Message}");
        }

        return new Dictionary<string, string>();
    }

    public void ClearSignUpDataAsync()
    {
        try
        {
            SecureStorage.Remove(String.SignUpDataStorageKey);
            _logger.LogInformation("Form data cleared from SecureStorage");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error clearing form data: {ex.Message}");
        }
    }
}