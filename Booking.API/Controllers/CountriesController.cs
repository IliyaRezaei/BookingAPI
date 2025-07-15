using Booking.Domain.Abstractions.Services.Manager;
using Booking.Domain.Contracts.Amenity;
using Booking.Domain.Contracts.Country;
using Booking.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Booking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;
        public CountriesController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var countries = await _serviceManager.Countries.GetAll();
            return Ok(countries);
        }
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var country = await _serviceManager.Countries.GetById(id);
            return Ok(country);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCountryRequest request)
        {
            var result = await _serviceManager.Countries.Create(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        [HttpPost("{id:guid}/image")]
        public async Task<IActionResult> UploadImage([FromRoute] Guid id, [FromForm] IFormFile image)
        {
            var imageUrl = await _serviceManager.ImageStorage.StoreImageAsync(image, EntityTypeImage.Country, id, Directory.GetCurrentDirectory());
            return Ok(imageUrl);
        }
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateCountryRequest request)
        {
            await _serviceManager.Countries.Update(request, id);
            return NoContent();
        }
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            await _serviceManager.Countries.Delete(id);
            return NoContent();
        }
    }
}
