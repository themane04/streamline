using System.Text.Json.Serialization;
using Streamline.Models;

namespace Streamline.Utilities;

public class MovieResponse
{
    [JsonPropertyName("results")] public List<Movie>? Results { get; set; }

    [JsonPropertyName("page")] public int Page { get; set; }

    [JsonPropertyName("total_pages")] public int TotalPages { get; set; }

    [JsonPropertyName("total_results")] public int TotalResults { get; set; }
}