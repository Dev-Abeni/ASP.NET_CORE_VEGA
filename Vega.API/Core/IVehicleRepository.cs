using System.Collections.Generic;
using System.Threading.Tasks;
using Vega.API.Core.Models;

namespace Vega.API.Core
{
    public interface IVehicleRepository
    {
        void Add(Vehicle vehicle);
        Task<Vehicle> GetVehicle(int id, bool includeRelated = true);
        Task<IEnumerable<Vehicle>> GetVehicles(VehicleQuery filter);
        void Remove(Vehicle vehicle);
        Task<Model> GetModelWithMake(Vehicle vehicle);
    }
}