using System.Text.Json.Serialization;

namespace Streamline.Models;

public class MovieDetail
{
    public string FullPosterPath => $"https://image.tmdb.org/t/p/w500{PosterPath}";
    public string FullBackdropPath => $"https://image.tmdb.org/t/p/w500{BackdropPath}";

    [JsonPropertyName("adult")] public bool Adult { get; set; }

    [JsonPropertyName("backdrop_path")] public string? BackdropPath { get; set; }

    [JsonPropertyName("belongs_to_collection")]
    public MovieCollection? BelongsToCollection { get; set; }

    [JsonPropertyName("budget")] public int Budget { get; set; }

    [JsonPropertyName("genres")] public List<MovieGenre> Genres { get; set; } = new();

    [JsonPropertyName("homepage")] public string? Homepage { get; set; }

    [JsonPropertyName("id")] public int Id { get; set; }

    [JsonPropertyName("imdb_id")] public string? ImdbId { get; set; }

    [JsonPropertyName("origin_country")] public List<string> OriginCountry { get; set; } = new();

    [JsonPropertyName("original_language")] public string OriginalLanguage { get; set; } = string.Empty;

    [JsonPropertyName("original_title")] public string OriginalTitle { get; set; } = string.Empty;
    
    [JsonPropertyName("overview")] public string Overview { get; set; } = string.Empty;

    [JsonPropertyName("popularity")] public double Popularity { get; set; }

    [JsonPropertyName("poster_path")] public string? PosterPath { get; set; }

    [JsonPropertyName("production_companies")]
    public List<MovieProductionCompany> ProductionCompanies { get; set; } = new();

    [JsonPropertyName("production_countries")]
    public List<MovieProductionCompany> ProductionCountries { get; set; } = new();

    [JsonPropertyName("release_date")] public string ReleaseDate { get; set; } = string.Empty;

    [JsonPropertyName("revenue")] public long Revenue { get; set; }

    [JsonPropertyName("runtime")] public int Runtime { get; set; }

    [JsonPropertyName("spoken_languages")] public List<MovieSpokenLanguage> SpokenLanguages { get; set; } = new();

    [JsonPropertyName("status")] public string Status { get; set; } = string.Empty;

    [JsonPropertyName("tagline")] public string? Tagline { get; set; }

    [JsonPropertyName("title")] public string Title { get; set; } = string.Empty;

    [JsonPropertyName("video")] public bool Video { get; set; }

    [JsonPropertyName("vote_average")] public double VoteAverage { get; set; }

    [JsonPropertyName("vote_count")] public int VoteCount { get; set; }
}