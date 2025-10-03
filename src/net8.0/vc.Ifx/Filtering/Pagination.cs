namespace vc.Ifx.Filtering;

/// <summary>
/// Handles pagination parameters for data queries.
/// </summary>
public class Pagination
{
    private int? skip;
    private int? take;

    /// <summary>
    /// Gets or sets the number of items to skip.
    /// </summary>
    public int? Skip
    {
        get => skip;
        set => skip = value < 0 ? null : value;
    }

    /// <summary>
    /// Gets or sets the number of items to take.
    /// </summary>
    public int? Take
    {
        get => take;
        set => take = value < 0 ? null : value;
    }

    /// <summary>
    /// Gets or sets the page number (1-based).
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Gets or sets the page size.
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Updates Skip and Take based on Page and PageSize.
    /// </summary>
    public void UpdateSkipTakeFromPage()
    {
        Skip = ( Page - 1 ) * PageSize;
        Take = PageSize;
    }

    /// <summary>
    /// Updates Page based on Skip and Take.
    /// </summary>
    public void UpdatePageFromSkipTake()
    {
        if(Take is not > 0)
            return;
        Page = ( Skip ?? 0 ) / Take.Value + 1;
        PageSize = Take.Value;
    }

    /// <summary>
    /// Clears pagination settings.
    /// </summary>
    public void Clear()
    {
        Skip = null;
        Take = null;
        Page = 1;
        PageSize = 10;
    }
}