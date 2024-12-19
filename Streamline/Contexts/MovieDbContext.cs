using Streamline.Models;
using Microsoft.EntityFrameworkCore;
using Streamline.Utilities;

namespace Streamline.Contexts;

public class MovieDbContext : DbContext
{
    public DbSet<MovieWatchlist> WatchlistMovies { get; set; }

    public MovieDbContext(DbContextOptions<MovieDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql(Environments.GetConnectionString());
        }
    }
}
