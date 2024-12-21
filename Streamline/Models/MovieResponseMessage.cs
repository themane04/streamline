using System.Text.Json.Serialization;

namespace Streamline.Models;

public class MovieResponseMessage
{
    [JsonPropertyName("message")] public string Message { get; set; } = "";
}