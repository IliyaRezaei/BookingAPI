using Booking.Domain.Contracts.Amenity;
using Booking.Domain.Contracts.User;
using Booking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Domain.Abstractions.Services
{
    public interface IUserService
    {
        public Task<IEnumerable<UserResponse>> GetAll();
        public Task<UserResponse> GetById(Guid id);
        public Task<UserResponse> Register(UserRegisterRequest request);
        public Task<string> Login(UserLoginRequest request);
        public Task Delete(Guid id, string username);
    }
}
