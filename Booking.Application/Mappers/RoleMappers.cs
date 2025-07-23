using Booking.Domain.Contracts.Role;
using Booking.Domain.Entities;

namespace Booking.Application.Mappers
{
    public static class RoleMappers
    {

        public static ApplicationRole ToEntity(this CreateRoleRequest request)
        {
            return new ApplicationRole
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                NormalizedName = request.Name.ToUpper()
            };
        }

        public static ApplicationRole ToEntity(this UpdateRoleRequest request, Guid id)
        {
            return new ApplicationRole
            {
                Id = id,
                Name = request.Name,
                NormalizedName = request.Name.ToUpper()
            };
        }

        public static RoleResponse ToResponse(this ApplicationRole role)
        {
            return new RoleResponse
            {
                Id = role.Id,
                Name = role.Name,
            };
        }

        public static IEnumerable<RoleResponse> ToResponse(this IEnumerable<ApplicationRole> roles)
        {
            return roles.Select(r => r.ToResponse()).ToList();
        }
    }
}
