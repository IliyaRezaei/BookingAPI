using Booking.Application.Utilities;
using Booking.Application.Validators.User;
using Booking.Domain.Abstractions.Repositories.Manager;
using Booking.Domain.Contracts.User;
using Booking.Domain.Entities;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.Tests.Unit.Validators.User
{
    public class UserLoginRequestValidatorTests
    {
        private readonly IRepositoryManager _repository;
        private readonly UserLoginRequestValidator _validator;
        public UserLoginRequestValidatorTests()
        {
            _repository = Substitute.For<IRepositoryManager>();
            _validator = new UserLoginRequestValidator(_repository);
        }

        [Theory]
        [InlineData("test@gmail.com", "Test1234@")]
        [InlineData("test@gmail", "Password")]
        public async Task ShouldPass_WhenValidInputs(string email, string password)
        {
            //Arrange
            var request = new UserLoginRequest { Email = email, Password = password };
            var user1 = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = email,
                NormalizedEmail = email.ToUpper(),
                Username = "username",
                NormalizedUsername = "USERNAME",
                HashedPassword = password.HashPassword(),
            };
            _repository.Users.GetByEmail(user1.Email).Returns(user1);
            //Act
            var result = await _validator.ValidateAsync(request);
            //Assert
            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData("test@gmail.com")]
        [InlineData("ilia@gmail.com")]
        [InlineData("test.com")]
        public async Task ShouldFail_WhenNoUserExistWithThatEmailOrInvalidEmail(string email)
        {
            //Arrange
            var request = new UserLoginRequest { Email = email, Password = "Test1234@" };
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = "email@gmail.com",
                NormalizedEmail = email.ToUpper(),
                Username = "username",
                NormalizedUsername = "USERNAME",
                HashedPassword = "Test1234@".HashPassword(),
            };
            _repository.Users.GetByEmail(user.Email).Returns(user);
            //Act
            var result = await _validator.ValidateAsync(request);
            //Assert
            Assert.False(result.IsValid);
        }
        [Theory]
        [InlineData("Test1234@")]
        [InlineData("Password")]
        public async Task ShouldFail_WhenValidButWrongPassword(string password)
        {
            //Arrange
            var request = new UserLoginRequest { Email = "validemail@gmail.com", Password = password };
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                NormalizedEmail = request.Email.ToUpper(),
                Username = "username",
                NormalizedUsername = "USERNAME",
                HashedPassword = ("test1234@").HashPassword(),
            };
            _repository.Users.GetByEmail(user.Email).Returns(user);
            //Act
            var result = await _validator.ValidateAsync(request);
            //Assert
            Assert.False(result.IsValid);
        }
    }
}
