using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vega.API.Controllers.Resources;
using Vega.API.Core;
using Vega.API.Core.Models;
using Vega.API.Persistance;

namespace Vega.API.Controllers
{
    // Attribute routing
    [Route("api/vehicles")]
    public class VehiclesController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IVehicleRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public VehiclesController(IMapper mapper, IVehicleRepository repository, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IEnumerable<VehicleResource>> GetVehicles(VehicleQueryResource fIlterResource)
        {
            var filter = _mapper.Map<VehicleQueryResource, VehicleQuery>(fIlterResource);
            var vehicles = await _repository.GetVehicles(filter);

            return _mapper.Map<IEnumerable<Vehicle>, IEnumerable<VehicleResource>>(vehicles);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVehicle(int id)
        {
            var vehicle = await _repository.GetVehicle(id);

            if (vehicle == null)
                return NotFound();

            var vehicleResource = _mapper.Map<Vehicle, VehicleResource>(vehicle);

            return Ok(vehicleResource);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateVehicle(SaveVehicleResource vehicleResource)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // This creates extra noise in the code, its better to use such validation
            // errors if we are working on public APIs, but on this case since
            // we are developing the backend to an Angular app this is neccessirly required

            //var model = await _context.Models.FindAsync(vehicleResource.ModelId);
            //if (model == null)
            //{
            //    ModelState.AddModelError("ModelId", "Invalid ModelId.");
            //    return BadRequest(ModelState);
            //}

            var vehicle = _mapper.Map<SaveVehicleResource, Vehicle>(vehicleResource);
            vehicle.LastUpdate = DateTime.Now;

            _repository.Add(vehicle);
            await _unitOfWork.CompleteAsync();

            await _repository.GetModelWithMake(vehicle);

            vehicle = await _repository.GetVehicle(vehicle.Id);

            var result = _mapper.Map<Vehicle, VehicleResource>(vehicle);

            return Ok(result);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateVehicle(int id, [FromBody] SaveVehicleResource vehicleResource)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var vehicle = await _repository.GetVehicle(id);

            if (vehicle == null)
                return NotFound();

            _mapper.Map<SaveVehicleResource, Vehicle>(vehicleResource, vehicle);
            vehicle.LastUpdate = DateTime.Now;

            await _unitOfWork.CompleteAsync();

            vehicle = await _repository.GetVehicle(vehicle.Id);
            var result = _mapper.Map<Vehicle, VehicleResource>(vehicle);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteVehicle(int id)
        {
            var vehicle = await _repository.GetVehicle(id, includeRelated: false);

            if (vehicle == null)
                return NotFound();

            _repository.Remove(vehicle);
            await _unitOfWork.CompleteAsync();

            return Ok(id);
        }
    }
}
