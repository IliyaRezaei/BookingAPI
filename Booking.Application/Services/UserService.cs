using Booking.Application.Mappers;
using Booking.Application.Validators.User;
using Booking.Domain.Abstractions.Repositories.Manager;
using Booking.Domain.Abstractions.Services;
using Booking.Domain.Contracts.User;
using Booking.Domain.Entities;
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
            var validator = new UserRegisterRequestValidator();
            var result = validator.Validate(request);
            if (result.IsValid)
            {
                throw new Exception("is valid");
            }
            var users = await _repositoryManager.Users.GetAll();
            if(users.Any(u => u.Email == request.Email) || users.Any(u => u.Username == request.Username))
            {
                throw new Exception("Email or Username is already taken");
            }
            var userRole = await _repositoryManager.Roles.GetByName("USER");
            var newUser = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                Username = request.Username,
                HashedPassword = HashPassword(request.Password),
            };
            if (userRole == null)
            {
                await _repositoryManager.Roles.Create(new ApplicationRole { Id = Guid.NewGuid(), Name = "USER" });
                await _repositoryManager.SaveAsync();
                userRole = await _repositoryManager.Roles.GetByName("USER");
            }
            await _repositoryManager.Users.Create(newUser);
            if (userRole != null)
            {
                newUser.Roles.Add(userRole);
            }
            await _repositoryManager.SaveAsync();
            return newUser.ToResponse();
        }

        public async Task<string> Login(UserLoginRequest request)
        {
            var user = await ValidateUser(request);
            if (user == null)
            {
                throw new Exception("provided credentials are incorrect");
            }
            var token = GenerateToken(user);
            return token;
        }

        public string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                var hashedPassword = Convert.ToBase64String(bytes);
                return hashedPassword;
            }
        }

        public async Task<ApplicationUser> ValidateUser(UserLoginRequest request)
        {
            var user = await _repositoryManager.Users.GetByEmail(request.Email);
            if(user != null && user.HashedPassword == HashPassword(request.Password))
            {
                return user;
            }
            return null;
        }

        public string GenerateToken(ApplicationUser user)
        {
            var key = _configuration["Jwt:Key"];
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            var tokenValidityMins = _configuration.GetValue<int>("Jwt:TokenValidityMins");
            var tokenExpireyDate = DateTime.UtcNow.AddMinutes(tokenValidityMins);
            // make login with email only so username would not be a security risk to put in the token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Username)
                }),
                Expires = tokenExpireyDate,
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
                                                            ,SecurityAlgorithms.HmacSha512Signature),

            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(securityToken);

            return accessToken;
        }

        public async Task Delete(Guid id)
        {
            var user = await _repositoryManager.Users.GetById(id);
            if (user == null)
            {
                throw new Exception("user not found");
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
                throw new Exception("user not found");
            }
            return user.ToResponse();
        }
    }
}
