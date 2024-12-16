using System.Text.Json.Serialization;

namespace Streamline.Models;

public class MovieProductionCompany
{
    [JsonPropertyName("id")] public int Id { get; set; }

    [JsonPropertyName("logo_path")] public string? LogoPath { get; set; }

    [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;

    [JsonPropertyName("origin_country")] public string OriginCountry { get; set; } = string.Empty;
}