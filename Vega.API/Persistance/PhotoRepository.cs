using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vega.API.Core;
using Vega.API.Core.Models;

namespace Vega.API.Persistance
{
    public class PhotoRepository : IPhotoRepository
    {
        private readonly VegaDbContext _context;

        public PhotoRepository(VegaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Photo>> GetPhotos(int vehicleId)
        {
            return await _context.Photos
                .Where(p => p.VehicleId == vehicleId)
                .ToListAsync();
        }
    }
}
