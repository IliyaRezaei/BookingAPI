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
    public class UserRegisterRequestValidatorTests
    {
        private readonly IRepositoryManager _repository;
        private readonly UserRegisterRequestValidator _validator;
        public UserRegisterRequestValidatorTests()
        {
            _repository = Substitute.For<IRepositoryManager>();
            _validator = new UserRegisterRequestValidator(_repository);
        }

        [Theory]
        [InlineData("test@gmail.com", "test", "Test1234@", "Test1234@")]
        [InlineData("test@gmail", "test1234", "Test1234@", "Test1234@")]
        public async Task ShouldPass_WhenValidInputs(
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
            //Act
            var result = await _validator.ValidateAsync(request);
            //Asert
            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData("test.com")]
        [InlineData("testemail")]
        [InlineData("email@gmail.com")]
        public async Task ShouldFail_WhenInvalidEmailOrAlreadyUsedEmail(string email)
        {
            //Arrange
            var request = new UserRegisterRequest
            {
                Email = email,
                Username = "ValidUsername",
                Password = "ValidPassword",
                ConfirmPassword = "ValidPassword"
            };
            var expectedUser = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Username = "username",
                NormalizedUsername = "USERNAME",
                Email = "email@gmail.com",
                NormalizedEmail = "EMAIL@GMAIL.COM",
                HashedPassword = "Test1234@".HashPassword()
            };
            _repository.Users.GetByUsername(expectedUser.Username).Returns(expectedUser);
            _repository.Users.GetByEmail(expectedUser.Email).Returns(expectedUser);
            //Act
            var result = await _validator.ValidateAsync(request);
            //Asert
            Assert.False(result.IsValid);
        }
        [Theory]
        [InlineData("username")]
        [InlineData("us")]
        [InlineData("usernameistooloooooooooooooooong")]
        public async Task ShouldFail_WhenInvalidUsernameOrAlreadyUsedUsername(string username)
        {
            //Arrange
            var request = new UserRegisterRequest
            {
                Email = "validemail@gmail.com",
                Username = username,
                Password = "ValidPassword",
                ConfirmPassword = "ValidPassword"
            };
            var expectedUser = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Username = "username",
                NormalizedUsername = "USERNAME",
                Email = "email@gmail.com",
                NormalizedEmail = "EMAIL@GMAIL.COM",
                HashedPassword = "Test1234@".HashPassword()
            };
            _repository.Users.GetByUsername(expectedUser.Username).Returns(expectedUser);
            _repository.Users.GetByEmail(expectedUser.Email).Returns(expectedUser);
            //Act
            var result = await _validator.ValidateAsync(request);
            //Asert
            Assert.False(result.IsValid);
        }
        [Theory]
        [InlineData("Invalid", "Invalid")]
        [InlineData("", "")]
        [InlineData(null, null)]
        public async Task ShouldFail_WhenInvalidPassword(string password, string confirmPassword)
        {
            //Arrange
            var request = new UserRegisterRequest
            {
                Email = "validemail@gmail.com",
                Username = "ValidUsername",
                Password = password,
                ConfirmPassword = confirmPassword
            };
            //Act
            var result = await _validator.ValidateAsync(request);
            //Asert
            Assert.False(result.IsValid);
        }
    }
}
