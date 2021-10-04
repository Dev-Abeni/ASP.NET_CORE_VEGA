using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Vega.API.Core;
using Vega.API.Core.Models;
using Vega.API.Extensions;

namespace Vega.API.Persistance
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly VegaDbContext _context;

        public VehicleRepository(VegaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Vehicle>> GetVehicles(VehicleQuery queryObj)
        {
            // Querying with filtering on the Server
            var query = _context.Vehicles
                .Include(v => v.Model)
                    .ThenInclude(m => m.Make)
                .Include(v => v.Features)
                    .ThenInclude(vf => vf.Feature)
                .AsQueryable();

            if (queryObj.MakeId.HasValue)
                query = query.Where(v => v.Model.MakeId == queryObj.MakeId);

            // Poor way to write the "SORT" code.
            //if (queryObj.SortBy == "make")
            //    query = (queryObj.IsSortAscending) ? query.OrderBy(v => v.Model.Make.Name) : query.OrderByDescending(v => v.Model.Make.Name);
            //if (queryObj.SortBy == "model")
            //    query = (queryObj.IsSortAscending) ? query.OrderBy(v => v.Model.Name) : query.OrderByDescending(v => v.Model.Name);

            // Better way to wrtie the "SORT" code.
            var columnsMap = new Dictionary<string, Expression<Func<Vehicle, object>>>()
            {
                ["make"] = v => v.Model.Make.Name,
                ["model"] = v => v.Model.Name, 
                ["contactName"] = v => v.ContactName
            };
            query = query.ApplyOrdering(queryObj, columnsMap);

            query = query.ApplyPaging(queryObj);

            return await query.ToListAsync();
        }

        public async Task<Vehicle> GetVehicle(int id, bool includeRelated = true)
        {
            if (!includeRelated)
                return await _context.Vehicles.FindAsync(id);
            
            return await _context.Vehicles
                .Include(v => v.Features)
                    // To specify nested responsed
                    .ThenInclude(vf => vf.Feature)
                .Include(v => v.Model)
                    .ThenInclude(m => m.Make)
                .SingleOrDefaultAsync(v => v.Id == id);
        }

        public void Add(Vehicle vehicle)
        {
            _context.Vehicles.Add(vehicle);
        }

        public void Remove(Vehicle vehicle)
        {
            _context.Vehicles.Remove(vehicle);
        }

        public async Task<Model> GetModelWithMake(Vehicle vehicle)
        {
            return await _context.Models.Include(m => m.Make).SingleOrDefaultAsync(m => m.Id == vehicle.ModelId);
        }
    }
}
