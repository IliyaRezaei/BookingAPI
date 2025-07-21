using Booking.Application.Services;
using Booking.Domain.Abstractions.Repositories.Manager;
using Booking.Domain.Contracts.User;
using Microsoft.Extensions.Configuration;
using Castle.Core.Configuration;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;
using Booking.Domain.Entities;
using Booking.Application.Utilities;
using FluentValidation;
using Booking.Application.Errors;

namespace Booking.Application.Tests.Unit.Services
{
    public class UserServiceTests
    {
        private readonly IRepositoryManager _repository;
        private readonly UserService _service;

        public UserServiceTests()
        {
            _repository = Substitute.For<IRepositoryManager>();
            _service = new UserService(_repository, GetMockConfiguration());
        }

        [Fact]
        public async Task Register_ReturnsUserResponse_WhenValidRequest()
        {
            //Arrange
            var request = new UserRegisterRequest
            {
                Email = "test@gmail.com",
                Username = "Test",
                Password = "Test1234@",
                ConfirmPassword = "Test1234@"
            };
            var role = new ApplicationRole
            {
                Id = Guid.NewGuid(),
                Name = "User",
                NormalizedName = "USER"
            };
            _repository.Users.GetByEmail(request.Email).Returns(Task.FromResult<ApplicationUser>(null));
            _repository.Users.GetByUsername(request.Username).Returns(Task.FromResult<ApplicationUser>(null));
            _repository.Roles.GetByName(role.Name).Returns(Task.FromResult<ApplicationRole>(role));

            //Act
            var userResponse = await _service.Register(request);

            //Assert
            await _repository.Users.Received(1).GetByEmail(request.Email);
            await _repository.Users.Received(1).GetByUsername(request.Username);
            Assert.NotNull(userResponse);
            Assert.IsType<UserResponse>(userResponse);
            Assert.IsType<Guid>(userResponse.Id);
            Assert.NotNull(userResponse.Email);
            Assert.NotNull(userResponse.Username);
        }

        [Fact]
        public async Task Register_ThrowsForbiddenException_WhenThereIsNoRoleNamedUSER()
        {
            //Arrange
            var request = new UserRegisterRequest
            {
                Email = "test@gmail.com",
                Username = "Test",
                Password = "Test1234@",
                ConfirmPassword = "Test1234@"
            };
            _repository.Users.GetByEmail(request.Email).Returns(Task.FromResult<ApplicationUser>(null));
            _repository.Users.GetByUsername(request.Username).Returns(Task.FromResult<ApplicationUser>(null));

            //Act
            var exception = await Assert.ThrowsAsync<ForbiddenException>(async () => await _service.Register(request));

            //Assert
            await _repository.Users.Received(1).GetByEmail(request.Email);
            await _repository.Users.Received(1).GetByUsername(request.Username);
            Assert.NotNull(exception);
            Assert.IsType<ForbiddenException>(exception);
        }

        [Theory]
        [InlineData("InvalidEmail.com","ValidUsername","ValidPassword1234@","ValidPassword1234@")]
        [InlineData("ValidEmail@gmail.com", "IV", "ValidPassword1234@", "ValidPassword1234@")]
        [InlineData("ValidEmail@gmail.com","ValidUsername", "1234" , "1234")]
        [InlineData("ValidEmail@gmail.com","ValidUsername", "ValidPassword1234@", "1234")]
        [InlineData("ValidEmail@gmail.com","ValidUsername", "1234" , "ValidPassword1234@")]
        public async Task Register_ThrowsValidationException_WhenInvalidParameters(
            string email, string username, 
            string password, string confirmPassword)
        {
            //Arrange
            var request = new UserRegisterRequest
            {
                Email = email,
                Username = username,
                Password = password,
                ConfirmPassword = confirmPassword
            };
            var role = new ApplicationRole
            {
                Id = Guid.NewGuid(),
                Name = "User",
                NormalizedName = "USER"
            };
            _repository.Users.GetByEmail(request.Email).Returns(Task.FromResult<ApplicationUser>(null));
            _repository.Users.GetByUsername(request.Username).Returns(Task.FromResult<ApplicationUser>(null));
            _repository.Roles.GetByName(role.Name).Returns(Task.FromResult<ApplicationRole>(role));

            //Act
            var exception = await Assert.ThrowsAsync<ValidationException>(async () => await _service.Register(request));

            //Assert
            await _repository.Users.Received(1).GetByEmail(request.Email);
            await _repository.Users.Received(1).GetByUsername(request.Username);
            Assert.NotNull(exception);
            Assert.IsType<ValidationException>(exception);
        }

        [Theory]
        [InlineData("Test1@gmail.com","Something")]
        [InlineData("Something@gmail.com","Test2")]
        public async Task Register_ThrowsValidationException_WhenUsernameOrEmailIsDuplicated(
            string email, string username)
        {
            //Arrange
            var request = new UserRegisterRequest
            {
                Email = email,
                Username = username,
                Password = "Test1234@",
                ConfirmPassword = "Test1234@"
            };

            _repository.Users.GetByEmail(request.Email).Returns(
                Task.FromResult<ApplicationUser>(GetUsers().FirstOrDefault(x => x.Email == request.Email)));

            _repository.Users.GetByUsername(request.Username).Returns(
                Task.FromResult<ApplicationUser>(GetUsers().FirstOrDefault(x => x.Username == request.Username)));

            //Act
            var exception = await Assert.ThrowsAsync<ValidationException>(async () => await _service.Register(request));

            //Assert
            await _repository.Users.Received(1).GetByEmail(request.Email);
            await _repository.Users.Received(1).GetByUsername(request.Username);
            Assert.NotNull(exception);
            Assert.IsType<ValidationException>(exception);
        }

        [Theory]
        [InlineData("Test1@gmail.com", "Test1234@")]
        [InlineData("TeSt2@gmail.com", "Test1234@")]
        [InlineData("TesT3@gmail.com", "Test1234@")]
        public async Task Login_ReturnsJWT_WhenValidParameters(string email, string password)
        {
            //Arrange
            var request = new UserLoginRequest
            {
                Email = email,
                Password = password,
            };
            _repository.Users.GetByEmail(request.Email).Returns(
                Task.FromResult<ApplicationUser>(GetUsers().FirstOrDefault(x => x.NormalizedEmail == request.Email.ToUpper())));
            _repository.Users.GetByEmail(request.Email.ToUpper()).Returns(
                Task.FromResult<ApplicationUser>(GetUsers().FirstOrDefault(x => x.Email == request.Email)));
            //Act
            var token = await _service.Login(request);


            //Assert
            Assert.NotNull(token);
            Assert.IsType<string>(token);
        }

        [Theory]
        [InlineData("Test1@gmail.com", "TeST1234@")]
        [InlineData("test.com", "Test1234@")]
        public async Task Login_ThrowsValidationException_WhenInvalidEmailOrPassword(string email, string password)
        {
            //Arrange
            var request = new UserLoginRequest
            {
                Email = email,
                Password = password
            };
            _repository.Users.GetAll().Returns(Task.FromResult<IEnumerable<ApplicationUser>>(GetUsers()));

            //Act
            var exception = await Assert.ThrowsAsync<ValidationException>(async () => await _service.Login(request));

            //Assert
            Assert.NotNull(exception);
            Assert.IsType<ValidationException>(exception);
        }

        private IConfiguration GetMockConfiguration()
        {
            var configs = new Dictionary<string, string>
            {
                { "Jwt:Key", "ApplicationJsonWebTokenSecretKeyThatHasToBeLongEnoughSoItCanActuallyWorkHopefully" },
                { "Jwt:Issuer", "https://localhost:7550" },
                { "Jwt:Audience", "https://localhost:8000" },
            };
            return new ConfigurationBuilder().AddInMemoryCollection(configs).Build();
        }

        private IEnumerable<ApplicationUser> GetUsers()
        {
            var users = new List<ApplicationUser>();
            users.Add(new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Username = "Test1",
                NormalizedUsername = "TEST1",
                Email = "Test1@gmail.com",
                NormalizedEmail = "TEST1@GMAIL.COM",
                HashedPassword = "Test1234@".HashPassword()
            });
            users.Add(new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Username = "Test2",
                NormalizedUsername = "TEST2",
                Email = "Test2@gmail.com",
                NormalizedEmail = "TEST2@GMAIL.COM",
                HashedPassword = "Test1234@".HashPassword()
            });
            users.Add(new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Username = "Test3",
                NormalizedUsername = "TEST3",
                Email = "Test3@gmail.com",
                NormalizedEmail = "TEST3@GMAIL.COM",
                HashedPassword = "Test1234@".HashPassword()
            });
            return users;
        }
    }
}
