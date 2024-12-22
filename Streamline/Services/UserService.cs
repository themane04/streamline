namespace Streamline.Services;

public class UserService
{
    private readonly HttpClient _httpClient;

    public UserService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<User>
}