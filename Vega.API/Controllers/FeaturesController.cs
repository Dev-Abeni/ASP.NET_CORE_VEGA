using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vega.API.Controllers.Resources;
using Vega.API.Core.Models;
using Vega.API.Persistance;

namespace Vega.API.Controllers
{
    public class FeaturesController : Controller
    {
        private readonly VegaDbContext _context;
        private readonly IMapper _mapper;
        public FeaturesController(VegaDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        [HttpGet("/api/features")]
        public async Task<IEnumerable<KeyValuePairResource>> GetFeatures()
        {
            var features = await _context.Features.ToListAsync();
            return _mapper.Map<List<Feature>, List<KeyValuePairResource>>(features);
        }
    }
}
