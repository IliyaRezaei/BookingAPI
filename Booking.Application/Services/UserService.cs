using Booking.Application.Errors;
using Booking.Application.Mappers;
using Booking.Application.Utilities;
using Booking.Application.Validators.User;
using Booking.Domain.Abstractions.Repositories.Manager;
using Booking.Domain.Abstractions.Services;
using Booking.Domain.Contracts.User;
using Booking.Domain.Entities;
using Booking.Domain.Errors;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IConfiguration _configuration;
        public UserService(IRepositoryManager repositoryManager, IConfiguration configuration)
        {
            _repositoryManager = repositoryManager;
            _configuration = configuration;
        }

        public async Task<UserResponse> Register(UserRegisterRequest request)
        {
            var validator = new UserRegisterRequestValidator(_repositoryManager);
            await validator.ValidateAndThrowAsync(request);

            var role = await _repositoryManager.Roles.GetByName("User");
            var user = request.ToEntity();
            if (role == null)
            {
                throw new ForbiddenException($"User registeration is down, contact admin");
                //throw new NotFoundException($"Role with name USER not found");
            }
            user.Roles.Add(role);
            await _repositoryManager.Users.Create(user);
            await _repositoryManager.SaveAsync();
            return user.ToResponse();
        }

        public async Task<string> Login(UserLoginRequest request)
        {
            var validator = new UserLoginRequestValidator(_repositoryManager);
            await validator.ValidateAndThrowAsync(request);

            var user = await _repositoryManager.Users.GetByEmail(request.Email) ??
                throw new UnauthorizedException("Login and try again");

            var token = AuthExtensions.GenerateToken(_configuration, user);
            return token;
        }

        public async Task Delete(Guid id, string username)
        {
            var user = await _repositoryManager.Users.GetById(id) ??
                throw new NotFoundException($"User with id {id} not found");

            if (user.Username != username && user.Roles.Any(x => x.Name != "ADMIN"))
            {
                throw new UnauthorizedException(":)");
            }
            _repositoryManager.Users.Delete(user);
            await _repositoryManager.SaveAsync();
        }

        public async Task<IEnumerable<UserResponse>> GetAll()
        {
            var users = await _repositoryManager.Users.GetAll();
            return users.ToResponse();
        }

        public async Task<UserResponse> GetById(Guid id)
        {
            var user = await _repositoryManager.Users.GetById(id);
            if (user == null)
            {
                throw new NotFoundException($"User with id {id} not found");
            }
            return user.ToResponse();
        }
    }
}
