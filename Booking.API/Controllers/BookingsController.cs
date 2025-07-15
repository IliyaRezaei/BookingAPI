using Booking.Domain.Abstractions.Services.Manager;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Booking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public BookingsController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var bookings = _serviceManager.Bookings.GetAll();
            return Ok(bookings);
        }

        [HttpGet("{id:guid}", Name = "GetBookingById")]
        public async Task<IActionResult> GetById([FromRoute]Guid id)
        {
            var booking = await _serviceManager.Bookings.GetById(id);
            return Ok(booking);
        }
    }
}
