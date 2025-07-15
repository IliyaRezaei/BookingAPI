using Booking.Domain.Abstractions.Services.Manager;
using Booking.Domain.Contracts.Role;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Booking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public RolesController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var roles = await _serviceManager.Roles.GetAll();
            return Ok(roles);
        }
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute]Guid id)
        {
            var role = await _serviceManager.Roles.GetById(id);
            return Ok(role);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody]CreateRoleRequest request)
        {
            var role = await _serviceManager.Roles.Create(request);
            return CreatedAtAction("GetById", new { id = role.Id }, role);
        }
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update([FromRoute]Guid id ,[FromBody]UpdateRoleRequest request)
        {
            await _serviceManager.Roles.Update(request, id);
            return Ok();    
        }
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            await _serviceManager.Roles.Delete(id);
            return NoContent();
        }
    }
}
