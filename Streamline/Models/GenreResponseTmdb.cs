using System.Text.Json.Serialization;

namespace Streamline.Models;

public class GenreResponseTmdb
{
    [JsonPropertyName("genres")] public List<MovieGenre> Genres { get; set; } = new();
}