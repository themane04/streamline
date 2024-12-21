using System.Text.Json.Serialization;

namespace Streamline.Models;

public class MovieShort
{
    public string FullPosterPath => $"https://image.tmdb.org/t/p/w500{PosterPath}";
    [JsonPropertyName("id")] public int Id { get; set; }
    [JsonPropertyName("movie_id")] public int MovieId { get; set; }
    [JsonPropertyName("title")] public string Title { get; set; } = null!;
    [JsonPropertyName("poster_path")] public string PosterPath { get; set; } = null!;
    [JsonPropertyName("vote_average")] public double VoteAverage { get; set; }
    [JsonPropertyName("is_watchlisted")] public bool IsWatchlisted { get; set; }
    [JsonPropertyName("is_favorite")] public bool IsFavorite { get; set; }
}