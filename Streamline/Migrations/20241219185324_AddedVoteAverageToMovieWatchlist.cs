using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Streamline.Migrations
{
    /// <inheritdoc />
    public partial class AddedVoteAverageToMovieWatchlist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "VoteAverage",
                table: "WatchlistMovies",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VoteAverage",
                table: "WatchlistMovies");
        }
    }
}
