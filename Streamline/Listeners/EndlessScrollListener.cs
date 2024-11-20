using AndroidX.RecyclerView.Widget;
using Streamline.Enums;
using Streamline.Utilities;

namespace Streamline.Listeners;

public class EndlessScrollListener : RecyclerView.OnScrollListener
{
    private bool _isLoading;
    private int _currentPage;
    private readonly GridLayoutManager _layoutManager;
    private readonly Func<int, Task> _loadMoreCallback;

    public EndlessScrollListener(GridLayoutManager layoutManager, Func<int, Task> loadMoreCallback)
    {
        _layoutManager = layoutManager ?? throw new ArgumentNullException(nameof(layoutManager));
        _loadMoreCallback = loadMoreCallback ?? throw new ArgumentNullException(nameof(loadMoreCallback));
        _currentPage = 1;
        _isLoading = false;
    }

    public override async void OnScrolled(RecyclerView recyclerView, int dx, int dy)
    {
        base.OnScrolled(recyclerView, dx, dy);

        if (dy > 0)
        {
            int visibleItemCount = recyclerView.ChildCount;
            int totalItemCount = _layoutManager.ItemCount;
            int firstVisibleItemPosition = _layoutManager.FindFirstVisibleItemPosition();

            if (!_isLoading && (visibleItemCount + firstVisibleItemPosition >= totalItemCount - 5))
            {
                _isLoading = true;
                _currentPage++;

                LogHelper.Log(LogLevel.Info, "EndlessScrollListener", $"Loading page {_currentPage}.");

                await _loadMoreCallback(_currentPage);

                _isLoading = false;
            }
        }
    }
}
