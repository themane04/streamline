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
        }
    }

    public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
    {
        var inflater = LayoutInflater.From(parent.Context);
        View? itemView = inflater?.Inflate(Resource.Layout.movie_item, parent, false);

        if (itemView == null)
        {
            throw new InvalidOperationException("LayoutInflater returned a null view.");
        }

        return new MovieViewHolder(itemView);
    }

    private class MovieViewHolder : RecyclerView.ViewHolder
    {
        public ImageView? Poster { get; }

        public MovieViewHolder(View itemView) : base(itemView)
        {
            Poster = itemView.FindViewById<ImageView>(Resource.Id.moviePoster);

            if (Poster == null)
            {
                throw new InvalidOperationException("View components not found");
            }
        }
    }
}