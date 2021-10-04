using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Vega.API.Controllers.Resources
{
    public class PhotoResource
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public int VehicleId { get; set; }
    }
}
