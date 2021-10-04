using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vega.API.Extensions;

namespace Vega.API.Core.Models
{
    public class VehicleQuery : IQueryObject
    {
        // Properties for FILTERING
        public int? MakeId { get; set; }

        // Properties for SORTING
        public string SortBy { get; set; }
        public bool IsSortAscending { get; set; }

        // Properties for PAGINATION
        public int Page { get; set; }
        public byte PageSize { get; set; }
    }
}
