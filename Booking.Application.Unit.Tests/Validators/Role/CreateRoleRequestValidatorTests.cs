using Booking.Application.Validators.Role;
using Booking.Domain.Abstractions.Repositories.Manager;
using Booking.Domain.Contracts.Role;
using Booking.Domain.Entities;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.Tests.Unit.Validators.Role
{
    public class CreateRoleRequestValidatorTests
    {
        private readonly IRepositoryManager _repository;
        private readonly CreateRoleRequestValidator _validator;
        public CreateRoleRequestValidatorTests()
        {
            _repository = Substitute.For<IRepositoryManager>();
            _validator = new CreateRoleRequestValidator(_repository);
        }

        [Theory]
        [InlineData("Role")]
        [InlineData("Role 20 Characters")]
        public async Task ShouldPass_WhenNameIsValid(string name)
        {
            //Arrange
            var request = new CreateRoleRequest { Name = name };
            //Act
            var result = await _validator.ValidateAsync(request);
            //Assert
            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("Roooooooooooooooooooooooooole")]
        [InlineData("Role")]
        public async Task ShouldPass_WhenNameIsNotValidOrNotUnique(string name)
        {
            //Arrange
            var request = new CreateRoleRequest { Name = name };
            var expectedRole = new ApplicationRole { Id = Guid.NewGuid(), Name = "Role", NormalizedName = "ROLE" };
            _repository.Roles.GetByName("Role").Returns(expectedRole);
            //Act
            var result = await _validator.ValidateAsync(request);
            //Assert
            Assert.False(result.IsValid);
        }
    }
}
