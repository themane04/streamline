using System.Text.Json.Serialization;

namespace Streamline.Models;

public class MovieSpokenLanguage
{
    [JsonPropertyName("english_name")] public string EnglishName { get; set; } = string.Empty;

    [JsonPropertyName("iso_639_1")] public string Iso639_1 { get; set; } = string.Empty;

    [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
}