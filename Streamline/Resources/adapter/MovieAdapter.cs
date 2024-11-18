using Android.Views;
using Android.Content;
using AndroidX.RecyclerView.Widget;
using Com.Squareup.Picasso;
using Streamline.Resources.model;

namespace Streamline.Resources.adapter;

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
            
            // Check if viewHolder properties are not null before assigning values
            if (viewHolder.Title != null)
                viewHolder.Title.Text = movie.Title ?? "No Title";

            if (viewHolder.Overview != null)
                viewHolder.Overview.Text = movie.Overview ?? "No Overview";

            if (viewHolder.Poster != null)
                Picasso.With(_context).Load(movie.FullPosterPath).Into(viewHolder.Poster);
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
        public TextView? Title { get; }
        public TextView? Overview { get; }
        public ImageView? Poster { get; }

        public MovieViewHolder(View itemView) : base(itemView)
        {
            Title = itemView.FindViewById<TextView>(Resource.Id.movieTitle);
            Overview = itemView.FindViewById<TextView>(Resource.Id.movieOverview);
            Poster = itemView.FindViewById<ImageView>(Resource.Id.moviePoster);

            if (Title == null || Overview == null || Poster == null)
            {
                throw new InvalidOperationException("View components not found");
            }
        }
    }
}
