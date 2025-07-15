using Booking.Domain.Contracts.Amenity;
using Booking.Domain.Contracts.Role;
using Booking.Domain.Contracts.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Domain.Abstractions.Services
{
    public interface IRoleService
    {
        public Task<IEnumerable<RoleResponse>> GetAll();
        public Task<RoleResponse> GetById(Guid id);
        public Task<RoleResponse> Create(CreateRoleRequest request);
        public Task Delete(Guid id);
        public Task Update(UpdateRoleRequest request, Guid id);
    }
}
