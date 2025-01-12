using System.Text.Json.Serialization;

namespace Streamline.Models;

public class AuthenticatedUser
{
    public required int Id { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    [JsonPropertyName("birthday")] public required string Birthday { get; set; }
    [JsonPropertyName("profile_image")] public required string ProfileImage { get; set; }
    [JsonPropertyName("date_joined")] public required string DateJoined { get; set; }
}