﻿using System.Text.Json.Serialization;

namespace Streamline.Models;

public class Movie
{
    public int Id { get; set; }

    [JsonPropertyName("original_title")] public string Title { get; set; } = string.Empty;

    public string Overview { get; set; } = string.Empty;

    [JsonPropertyName("poster_path")] public string PosterPath { get; set; } = string.Empty;

    [JsonPropertyName("vote_average")] public double VoteAverage { get; set; }

    [JsonPropertyName("vote_count")] public int VoteCount { get; set; }

    [JsonPropertyName("backdrop_path")] public string? BackdropPath { get; set; }

    [JsonPropertyName("release_date")] public string ReleaseDate { get; set; } = string.Empty;

    [JsonPropertyName("original_language")]
    public string OriginalLanguage { get; set; } = string.Empty;

    public double Popularity { get; set; }

    [JsonPropertyName("genre_ids")] public List<int> GenreIds { get; set; } = new();

    public bool Adult { get; set; }
    public bool Video { get; set; }

    public string FullPosterPath => $"https://image.tmdb.org/t/p/w500{PosterPath}";
    public string FullBackdropPath => $"https://image.tmdb.org/t/p/w500{BackdropPath}";
}