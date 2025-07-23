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
    public class UpdateRoleRequestValidatorTests
    {
        private readonly IRepositoryManager _repository;
        private readonly UpdateRoleRequestValidator _validator;
        private readonly ApplicationRole _role;
        public UpdateRoleRequestValidatorTests()
        {
            _repository = Substitute.For<IRepositoryManager>();
            _role = new ApplicationRole { Id = Guid.NewGuid(), Name = "Role", NormalizedName = "ROLE" };
            _validator = new UpdateRoleRequestValidator(_repository, _role.Id);
        }

        [Theory]
        [InlineData("Role")]
        [InlineData("Role 20 Characters")]
        public async Task ShouldPass_WhenNameIsValid(string name)
        {
            //Arrange
            var request = new UpdateRoleRequest { Name = name };
            _repository.Roles.GetByName(_role.Name).Returns(_role);
            //Act
            var result = await _validator.ValidateAsync(request);
            //Assert
            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("Roooooooooooooooooooooooooole")]
        [InlineData("Dup")]
        public async Task ShouldPass_WhenNameIsNotValidOrNotUnique(string name)
        {
            //Arrange
            var request = new UpdateRoleRequest { Name = name };
            var expectedRole = new ApplicationRole { Id = Guid.NewGuid(), Name = "Dup", NormalizedName = "DUP" };
            _repository.Roles.GetByName(expectedRole.Name).Returns(expectedRole);
            //Act
            var result = await _validator.ValidateAsync(request);
            //Assert
            Assert.False(result.IsValid);
        }
    }
}
