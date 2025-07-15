using Booking.Domain.Abstractions.Services.Manager;
using Booking.Domain.Contracts.Amenity;
using Booking.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Booking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AmenitiesController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;
        public AmenitiesController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var amenities = await _serviceManager.Amenities.GetAll();
            return Ok(amenities);
        }
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute]Guid id)
        {
            var amenity = await _serviceManager.Amenities.GetById(id);
            return Ok(amenity);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody]CreateAmenityRequest request)
        {
            var result = await _serviceManager.Amenities.Create(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        [HttpPost("{id:guid}/image")]
        public async Task<IActionResult> UploadImage([FromRoute]Guid id ,[FromForm] IFormFile image)
        {
            var imageUrl = await _serviceManager.ImageStorage.StoreImageAsync(image, EntityTypeImage.Amenity, id, Directory.GetCurrentDirectory());
            return Ok(imageUrl);
        }
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update([FromRoute]Guid id, [FromBody]UpdateAmenityRequest request)
        {
            await _serviceManager.Amenities.Update(request, id);
            return NoContent();
        }
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            await _serviceManager.Amenities.Delete(id);
            return NoContent();
        }
    }
}
