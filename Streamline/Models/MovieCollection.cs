using System.Text.Json.Serialization;

namespace Streamline.Models;

public class MovieCollection
{
    [JsonPropertyName("id")] public int Id { get; set; }

    [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;

    [JsonPropertyName("poster_path")] public string? PosterPath { get; set; }

    [JsonPropertyName("backdrop_path")] public string? BackdropPath { get; set; }
}