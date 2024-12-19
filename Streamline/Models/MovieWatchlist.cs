namespace Streamline.Models;

public class MovieWatchlist
{
    public int Id { get; set; }
    public int MovieId { get; set; }
    public string Title { get; set; } = null!;
    public string PosterPath { get; set; } = null!;
}