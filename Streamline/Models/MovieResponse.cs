using System.Text.Json.Serialization;

namespace Streamline.Models;

public class MovieResponse<T>
{
    [JsonPropertyName("code")] public int Code { get; set; }
    [JsonPropertyName("message")] public string Message { get; set; }
    [JsonPropertyName("endpoint")] public string Endpoint { get; set; }
    [JsonPropertyName("data")] public T Data { get; set; }
}

public class MovieResponseNoInterface
{
    [JsonPropertyName("code")] public int Code { get; set; }
    [JsonPropertyName("message")] public string Message { get; set; }
    [JsonPropertyName("endpoint")] public string Endpoint { get; set; }
    [JsonPropertyName("data")] public Object Data { get; set; }
}