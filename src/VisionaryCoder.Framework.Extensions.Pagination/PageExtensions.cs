using Microsoft.EntityFrameworkCore;
namespace VisionaryCoder.Framework.Extensions.Pagination;

public static class PageExtensions
{

    // Offset-based (simple, fine for small/medium datasets)
    public static async Task<Page<T>> ToPageAsync<T>(this IQueryable<T> query, PageRequest request, CancellationToken ct = default)
    {
        var count = await query.CountAsync(ct);
        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        return new Page<T>(items, count, request.PageNumber, request.PageSize);
    }

    // Token-based hook (for high-scale/unstable ordering; implement per store)
    public static Task<Page<T>> ToPageWithTokenAsync<T>(this IQueryable<T> query, PageRequest request, Func<IQueryable<T>, string?, int, CancellationToken, Task<(IReadOnlyList<T> Items, string? NextToken)>> pageFn, CancellationToken ct = default) => ExecuteAsync(query, request, pageFn, ct);

    static async Task<Page<T>> ExecuteAsync<T>(IQueryable<T> source, PageRequest request, Func<IQueryable<T>, string?, int, CancellationToken, Task<(IReadOnlyList<T>, string?)>> fn, CancellationToken ct)
    {
        var (items, next) = await fn(source, request.ContinuationToken, request.PageSize, ct);
        return new Page<T>(items, count: 0, pageNumber: 0, pageSize: request.PageSize, nextToken: next);
    }
}
