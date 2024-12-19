using System.ComponentModel.DataAnnotations;

namespace Streamline.Models;

public class MovieWatchlist
{
    public string FullPosterPath => $"https://image.tmdb.org/t/p/w500{PosterPath}";

    public int Id { get; set; }
    public int MovieId { get; set; }
    [StringLength(255)] public string Title { get; set; } = null!;
    [StringLength(255)] public string PosterPath { get; set; } = null!;
    public double VoteAverage { get; set; }
}