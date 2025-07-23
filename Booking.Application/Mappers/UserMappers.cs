using Booking.Application.Utilities;
using Booking.Domain.Contracts.User;
using Booking.Domain.Entities;

namespace Booking.Application.Mappers
{
    public static class UserMappers
    {
        public static ApplicationUser ToEntity(this UserRegisterRequest request)
        {
            return new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                NormalizedEmail = request.Email.ToUpper(),
                Username = request.Username,
                NormalizedUsername = request.Username.ToUpper(),
                HashedPassword = request.Password.HashPassword(),
            };
        }
        public static UserResponse ToResponse(this ApplicationUser user)
        {
            return new UserResponse
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.Username,
                ImageUrl = user.ImageUrl,
                IsHost = user.IsHost,
            };
        }
        public static IEnumerable<UserResponse> ToResponse(this IEnumerable<ApplicationUser> users)
        {
            return users.Select(u => u.ToResponse()).ToList();
        }
    }
}
