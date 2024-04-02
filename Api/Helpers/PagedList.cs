﻿using Microsoft.EntityFrameworkCore;

namespace Api.Helpers
{
    public class PagedList<T>:List<T>
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotlaPages { get; set; }
        public int TotalCount { get; set; }

        public PagedList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
        {
            TotalCount=count;
            TotlaPages=(int) Math.Ceiling(count/(double) pageSize);
            PageSize=pageSize;
            CurrentPage=pageNumber;
            AddRange(items);
        }

        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source , int pageNumber , int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageNumber-1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedList<T>(items, count, pageNumber, pageSize);

        }
    }
}
