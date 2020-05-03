using SugarCounter.Core.Shared;
using System.Linq;

namespace SugarCounter.DataAccess.Utils
{
    internal static class Paginator
    {
        public static IQueryable<TItem> Paginate<TItem>(this IQueryable<TItem> source, PaginationParams prms)
        {
            return source.Skip((prms.PageNumber - 1) * prms.ItemsPerPage).Take(prms.ItemsPerPage);
        }
    }
}
