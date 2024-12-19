using Streamline.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Streamline.Contexts;

public class MovieDbContext: DbContext
{
    public DbSet<MovieWatchlist> WatchlistMovies { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("YourConnectionString");
    }
}