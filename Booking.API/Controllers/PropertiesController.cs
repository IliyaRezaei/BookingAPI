using Booking.Domain.Abstractions.Services.Manager;
using Booking.Domain.Contracts.Booking;
using Booking.Domain.Contracts.Property;
using Booking.Domain.Contracts.Review;
using Booking.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Booking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropertiesController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public PropertiesController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var properties = await _serviceManager.Properties.GetAll();
            return Ok(properties);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute]Guid id) {
            var property = await _serviceManager.Properties.GetById(id);
            return Ok(property);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody]CreatePropertyRequest request)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var response = await _serviceManager.Properties.Create(request, username);
            return CreatedAtAction("GetById", new { id = response.Id}, response);
        }

        [HttpPost("{propertyId:guid}/amenities")]
        public async Task<IActionResult> AddAmenities([FromRoute] Guid propertyId, [FromBody]PropertyAmenitiesRequest request)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await _serviceManager.Properties.AddAmenities(request, propertyId, username);
            return Ok();
        }


        [HttpPost("{propertyId:guid}/addresses")]
        public async Task<IActionResult> AddAddress([FromRoute] Guid propertyId, [FromBody]PropertyAddressRequest request)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await _serviceManager.Properties.AddAddress(request, propertyId, username);
            return Ok();
        }

        [HttpPost("{propertyId:guid}/images")]
        public async Task<IActionResult> AddImages([FromRoute] Guid propertyId, [FromForm] IFormFileCollection images)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var imageUrl = await _serviceManager.ImageStorage.StoreImagesAsync(images, EntityTypeImage.Property, propertyId, Directory.GetCurrentDirectory());
            return Ok(imageUrl);
        }

        [HttpPost("{propertyId:guid}/reviews")]
        public async Task<IActionResult> ReviewAProperty([FromBody] CreateReviewRequest request, Guid propertyId)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var response = await _serviceManager.Reviews.Create(request, username, propertyId);
            return CreatedAtAction("GetReviewById", new { id = response.Id }, response);
        }

        [HttpPost("{propertyId:guid}/bookings")]
        public async Task<IActionResult> BookAProperty([FromBody] CreateBookingRequest request, Guid propertyId)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var response = await _serviceManager.Bookings.Create(request, username, propertyId);
            return CreatedAtAction("GetBookingById", new { id = response.Id }, response);
        }
    }
}
