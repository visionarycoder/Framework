namespace vc.v2.Ifx.Core
{
    public static class PaginationExtensions
    {

        public static bool IsValid(this IPagination pagination)
        {
            return pagination is { Skip: >= 0, Take: null or > 0 };
        }

        /// <summary>
        ///     Configures pagination for the filter using page number and size.
        /// </summary>
        /// <param name="item">Item implementing IPagination</param>
        /// <param name="pageNumber">The page number (1-based).</param>
        /// <param name="pageSize">The number of items per page.</param>
        public static void SetPageNumberAndSize(this IPagination item, int pageNumber, int pageSize)
        {
            if (pageNumber is < 1 || pageSize is < 1)
                return; // Return early if the input is not valid

            item.Skip = (pageNumber - 1) * pageSize;
            item.Take = pageSize;
        }

        /// <summary>
        ///     Configures offset and limit for the filter.
        /// </summary>
        /// <param name="item">Item implementing IPagination</param>
        /// <param name="offset">The number of items to skip.</param>
        /// <param name="limit">The maximum number of items to take.</param>
        public static void SetOffsetAndLimit(this IPagination item, int offset, int limit)
        {
            if (offset is < 0 || limit is < 1)
                return; // Return early if the input is not valid
            item.Skip = offset;
            item.Take = limit;
        }

        /// <summary>
        ///     Applies pagination to the query based on the specified filter.
        /// </summary>
        /// <typeparam name="T">The type of the entity being queried.</typeparam>
        /// <param name="query">The query to apply pagination to.</param>
        /// <param name="pagination"></param>
        /// <returns>The paginated query.</returns>
        public static IQueryable<T> ApplyPagination<T>(this IQueryable<T> query, IPagination pagination) where T : class
        {

            if (!pagination.IsValid())
                throw new ArgumentException("Invalid pagination.", nameof(pagination));

            var skip = pagination.Skip is > 0
                ? pagination.Skip
                : 0;
            query = query.Skip(skip);

            if (pagination.Take is > 0)
                query = query.Take(pagination.Take.Value);
            return query;
        }

    }
}


