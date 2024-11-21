using Android.Views;
using Android.Content;
using AndroidX.RecyclerView.Widget;
using Streamline.Enums;
using Streamline.Models;
using Streamline.Utilities;

namespace Streamline.Adapters;

public class MovieAdapter : RecyclerView.Adapter
{
    private readonly List<Movie> _movies;
    private readonly Context _context;

    public MovieAdapter(Context context, List<Movie> movies)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _movies = movies ?? new List<Movie>();
    }

    public override int ItemCount => _movies.Count;

    public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
    {
        if (holder is MovieViewHolder viewHolder)
        {
            var movie = _movies[position];
            LogHelper.Log(LogLevel.Info, "MovieAdapter", $"Binding movie: {movie.Title}");

            if (!string.IsNullOrEmpty(movie.FullPosterPath))
            {
                PicassoSingleton.GetInstance(_context)
                    .Load(movie.FullPosterPath)
                    .Into(viewHolder.Poster);
            }

            if (viewHolder.Rating != null)
            {
                viewHolder.Rating.Text = movie.VoteAverage.ToString("0.0");
            }
        }
    }

    public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
    {
        var inflater = LayoutInflater.From(parent.Context);
        var itemView = inflater?.Inflate(Resource.Layout.movie_item, parent, false);
        return new MovieViewHolder(itemView ?? throw new InvalidOperationException("View not found"));
    }

    public void UpdateMovies(List<Movie> newMovies)
    {
        _movies.Clear();
        _movies.AddRange(newMovies);
        NotifyDataSetChanged();

        LogHelper.Log(LogLevel.Info, "MovieAdapter",
            $"Updated {newMovies.Count} movies. RecyclerView ItemCount: {ItemCount}");
    }

    public void AppendMovies(List<Movie> newMovies)
    {
        int startPosition = _movies.Count;
        _movies.AddRange(newMovies);
        NotifyItemRangeInserted(startPosition, newMovies.Count);
    }

    private class MovieViewHolder : RecyclerView.ViewHolder
    {
        public ImageView? Poster { get; }
        public TextView? Rating { get; }

        public MovieViewHolder(View itemView) : base(itemView)
        {
            Poster = itemView.FindViewById<ImageView>(Resource.Id.moviePoster);
            Rating = itemView.FindViewById<TextView>(Resource.Id.movieRating);

            if (Poster == null || Rating == null)
            {
                throw new InvalidOperationException("View components not found");
            }
        }
    }
}