using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Vega.API.Controllers.Resources;
using Vega.API.Core;
using Vega.API.Core.Models;
using Vega.API.Persistance;

namespace Vega.API.Controllers
{
    [Route("/api/vehicles/{vehicleId}/photos")]
    public class PhotosController : Controller
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IPhotoRepository _photoRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly PhotoSettings _photoSettings;
        public PhotosController(
            IVehicleRepository vehicleRepository, 
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IOptionsSnapshot<PhotoSettings> options, 
            IPhotoRepository photoRepository)
        {
            _photoSettings = options.Value;
            _mapper = mapper;
            _vehicleRepository = vehicleRepository;
            _photoRepository = photoRepository;
            _unitOfWork = unitOfWork;
        } 
         
        [HttpGet]
        public async Task<IEnumerable<PhotoResource>> GetPhotos(int vehicleId)
        {
            var photos = await _photoRepository.GetPhotos(vehicleId);

            return _mapper.Map<IEnumerable<Photo>, IEnumerable<PhotoResource>>(photos);
        }

        [HttpPost]
        public async Task<IActionResult> Upload(int vehicleId, IFormFile file)
        {
            var vehicle = await _vehicleRepository.GetVehicle(vehicleId, includeRelated: false);
            if (vehicle == null)
                return NotFound();

            // Validations
            if (file == null) return BadRequest("No file.");
            if (file.Length == 0) return BadRequest("Empty file.");
            if (file.Length > _photoSettings.MaxBytes) return BadRequest("Maximum file size exceeded.");
            if (!_photoSettings.IsSupported(file.FileName))
                return BadRequest("Invalid file type.");

            var uploadsFolderPath = Path.Combine("uploads");
            if (!Directory.Exists(uploadsFolderPath))
                Directory.CreateDirectory(uploadsFolderPath);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolderPath, fileName);

            using(var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var photo = new Photo { FileName = fileName };
            vehicle.Photos.Add(photo);
            await _unitOfWork.CompleteAsync();

            return Ok(_mapper.Map<Photo, PhotoResource>(photo));
        }
    }
}
