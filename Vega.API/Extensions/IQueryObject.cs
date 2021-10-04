using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vega.API.Extensions
{
    public interface IQueryObject
    {
        // Properties for SORTING
        string SortBy { get; set; }
        bool IsSortAscending { get; set; }

        // Properties for PAGINATION
        int Page { get; set; }
        byte PageSize { get; set; }
    }
}
