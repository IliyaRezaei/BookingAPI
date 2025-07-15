using Booking.Domain.Abstractions.Services.Manager;
using Booking.Domain.Contracts.User;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Booking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public UsersController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _serviceManager.Users.GetAll();
            return Ok(users);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute]Guid id)
        {
            var user = await _serviceManager.Users.GetById(id);
            return Ok(user);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterRequest request)
        {
            var user = await _serviceManager.Users.Register(request);
            return CreatedAtAction("GetById", new { id = user.Id }, user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        {
            var token = await _serviceManager.Users.Login(request);
            return Ok(new { token });
        }
        /*
        [HttpGet("{refreshToken:string}")]
        public async Task<IActionResult> GetNewJwtWithRefreshToken([FromRoute] string refreshToken)
        {
            var newToken = await _serviceManager.Users.GetNewJwtWithRefreshToken(refreshToken);
            return Ok(new { token = newToken } );
        }
        */
    }
}
