using Azure.Core;
using Booking.Domain.Abstractions.Services.Manager;
using Booking.Domain.Contracts.Amenity;
using Booking.Domain.Contracts.City;
using Booking.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Booking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;
        public CitiesController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var cities = await _serviceManager.Cities.GetAll();
            return Ok(cities);
        }
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var city = await _serviceManager.Cities.GetById(id);
            return Ok(city);
        }
        [HttpPost("{countryId:guid}")]
        public async Task<IActionResult> Create([FromRoute]Guid countryId, [FromBody] CreateCityRequest request)
        {
            var result = await _serviceManager.Cities.Create(request, countryId);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        [HttpPost("{id:guid}/image")]
        public async Task<IActionResult> UploadImage([FromRoute] Guid id, [FromForm] IFormFile image)
        {
            var imageUrl = await _serviceManager.ImageStorage.StoreImageAsync(image, EntityTypeImage.City, id, Directory.GetCurrentDirectory());
            return Ok(imageUrl);
        }
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateCityRequest request)
        {
            await _serviceManager.Cities.Update(request, id);
            return NoContent();
        }
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            await _serviceManager.Cities.Delete(id);
            return NoContent();
        }
    }
}
