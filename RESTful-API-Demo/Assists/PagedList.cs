using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace RESTful_API_Demo.Assists
{
    public class PagedList<T> : List<T>
    {
        public PagedList(
            List<T> source,
            int currentPage,
            int pageSize,
            int totalCount)
        {
            this.AddRange(source);

            this.CurrentPage = currentPage;
            this.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            this.PageSize = pageSize;
            this.TotalCount = totalCount;
        }

        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
        {
            var totalCount = await source.CountAsync();
            var items = await source.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToListAsync();
            return new PagedList<T>(items, pageNumber, pageSize, totalCount);
        }

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public bool HasNext => this.CurrentPage < this.TotalPages;
        public bool HasPrevious => this.CurrentPage > 1;
    }
}
