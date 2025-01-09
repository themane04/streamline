using System.Text.Json.Serialization;

namespace Streamline.Models;

public class MovieProductionCountry
{
    [JsonPropertyName("iso_3166_1")] public string Iso3166_1 { get; set; } = string.Empty;

    [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
}