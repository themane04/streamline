using System.Text.Json.Serialization;

namespace Streamline.Models;

public class BackendResponse<T>
{
    [JsonPropertyName("code")] public int Code { get; set; }
    [JsonPropertyName("message")] public string Message { get; set; } = "";
    [JsonPropertyName("endpoint")] public string Endpoint { get; set; } = "";
    [JsonPropertyName("data")] public required T Data { get; set; }
}