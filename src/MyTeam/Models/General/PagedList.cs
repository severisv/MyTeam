using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyTeam.Models.General
{
    public class PagedList<T> : List<T>
    {
        public int Skip { get; }
        public int Take { get; }
        public int TotalCount { get; }
        public bool HasPrevious => Skip - Take >= 0;
        public bool HasNext => Skip + Take < TotalCount;
        public int SkipNext => Skip + Take;
        public int SkipPrevious => Skip - Take;

        public PagedList(IEnumerable<T> items, int skip, int take, int totalCount) : base(items)
        {
            Skip = skip;
            Take = take;
            TotalCount = totalCount;
        }
    }
}
