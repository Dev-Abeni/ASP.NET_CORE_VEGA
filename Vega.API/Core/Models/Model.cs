using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Vega.API.Core.Models
{
    public class Model
    {
        public int Id { get; set; } 
        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        public Make Make { get; set; }
        public int MakeId { get; set; }
    }
}
