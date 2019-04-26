using StrokeUnitsService.Models;
using System.Collections.Generic;
using System.Linq;

namespace StrokeUnitsService.Extensions
{
    public static class PagingExtensions
    {
        public static PagedSearch<T> Page<T>(this IEnumerable<T> items, int pageToGet = 1, int itemsPerPage = 20)
        {
            var skip = (pageToGet - 1) * itemsPerPage;
            var totalItems = items.Count();

            return new PagedSearch<T>
            {
                Items = items.Skip(skip).Take(itemsPerPage).ToList(),
                TotalItems = items.Count(),
                ItemsPerPage = itemsPerPage,
                CurrentPage = pageToGet,
                TotalPages = (totalItems / itemsPerPage) + (totalItems % itemsPerPage != 0 ? 1 : 0)
            };
        }
    }
}
