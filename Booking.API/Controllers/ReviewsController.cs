using Booking.Domain.Abstractions.Services.Manager;
using Booking.Domain.Contracts.Review;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Booking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;
        public ReviewsController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(Guid propertyId)
        {
            var reviews = await _serviceManager.Reviews.GetAll();
            return Ok(reviews);
        }

        [HttpGet("{id:guid}", Name = "GetReviewById")]
        public async Task<IActionResult> GetById([FromRoute] Guid id, Guid propertyId)
        {
            var review = await _serviceManager.Reviews.GetById(id);
            return Ok(review);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody]UpdateReviewRequest request, Guid propertyId)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            await _serviceManager.Reviews.Update(request, id, username, propertyId);
            return Ok();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteById([FromRoute] Guid id, Guid propertyId)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            await _serviceManager.Reviews.Delete(id, username);
            return NoContent();
        }
    }
}
