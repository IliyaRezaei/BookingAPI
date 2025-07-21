using Booking.Application.Mappers;
using Booking.Application.Services;
using Booking.Domain.Abstractions.Repositories.Manager;
using Booking.Domain.Contracts.Role;
using Booking.Domain.Entities;
using Booking.Domain.Errors;
using FluentValidation;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.Tests.Unit.Services
{
    public class RoleServiceTests
    {
        private readonly IRepositoryManager _repository;
        private readonly RoleService _service;

        public RoleServiceTests()
        {
            _repository = Substitute.For<IRepositoryManager>();
            _service = new RoleService(_repository);
            ExistingRole = new ApplicationRole { Id = Guid.NewGuid(), Name = "Role", NormalizedName = "ROLE" };
            AllRoles = new List<ApplicationRole>
            {
                new ApplicationRole { Id = Guid.NewGuid(), Name = "FirstRole", NormalizedName = "FIRSTROLE"},
                new ApplicationRole { Id = Guid.NewGuid(), Name = "SecondRole", NormalizedName = "SECONDROLE"},
                new ApplicationRole { Id = Guid.NewGuid(), Name = "ThirdRole", NormalizedName = "THIRDROLE"},
            };
        }

        [Fact]
        public async Task Create_ShouldReturnsRoleResponse_WhenValidRequest()
        {
            //Arrange
            var request = new CreateRoleRequest
            {
                Name = "User"
            };

            //Act
            var roleResponse = await _service.Create(request);

            //Assert
            _repository.Roles.Received(1).Create(Arg.Is<ApplicationRole>(
                x => x.Name == "User" &&
                x.NormalizedName == "USER"));
            Assert.NotNull(roleResponse);
            Assert.IsType<RoleResponse>(roleResponse);
            Assert.NotNull(roleResponse.Name);
        }

        [Fact]
        public async Task Create_ShouldThrowValidationException_WhenNameIsNotUnique()
        {
            //Arrange
            var request = new CreateRoleRequest
            {
                Name = "Role"
            };
            _repository.Roles.GetByName(ExistingRole.Name).Returns(ExistingRole);

            //Act
            var exception = await Assert.ThrowsAsync<ValidationException>(async () => await _service.Create(request));

            //Assert
            Assert.NotNull(exception);
            Assert.IsType<ValidationException>(exception);
        }

        [Fact]
        public async Task GetById_ShouldReturnRoleResponse_WhenRoleExists()
        {
            //Arrange
            _repository.Roles.GetById(ExistingRole.Id).Returns(Task.FromResult(ExistingRole));

            //Act
            var roleResponse = await _service.GetById(ExistingRole.Id);

            //Assert
            Assert.NotNull(roleResponse);
            Assert.IsType<RoleResponse>(roleResponse);
            Assert.NotNull(roleResponse.Name);
        }

        [Fact]
        public async Task GetById_ShouldThrowNotFoundException_WhenRoleDoesntExist()
        {
            //Arrange
            var roleId = Guid.NewGuid();
            _repository.Roles.GetById(roleId).Returns(Task.FromResult<ApplicationRole>(null));

            //Act
            var exception = await Assert.ThrowsAsync<NotFoundException>(async () => await _service.GetById(roleId));

            //Assert
            Assert.NotNull(exception);
            Assert.IsType<NotFoundException>(exception);
            Assert.Contains(roleId.ToString(), exception.ErrorMessage);
        }

        [Fact]
        public async Task DeleteById_ShouldDeleteRole_WhenRoleExists()
        {
            //Arrange
            _repository.Roles.GetById(ExistingRole.Id).Returns(ExistingRole);

            //Act
            await _service.Delete(ExistingRole.Id);

            //Assert
            _repository.Roles.Received(1).Delete(Arg.Is<ApplicationRole>(
                x => x.Id == ExistingRole.Id &&
                x.Name == ExistingRole.Name &&
                x.NormalizedName == ExistingRole.NormalizedName)
                );
            await _repository.Received(1).SaveAsync();
        }

        [Fact]
        public async Task DeleteById_ShouldThrowNotFoundException_WhenRoleDoesntExist()
        {
            //Arrange
            var roleId = Guid.NewGuid();
            _repository.Roles.GetById(roleId).Returns(Task.FromResult<ApplicationRole>(null));

            //Act
            var exception = await Assert.ThrowsAsync<NotFoundException>(async () => await _service.Delete(roleId));

            //Assert
            Assert.NotNull(exception);
            Assert.IsType<NotFoundException>(exception);
            Assert.Contains(roleId.ToString(), exception.ErrorMessage);
        }

        [Fact]
        public async Task Update_ShouldUpdate_WhenRoleExists()
        {
            //Arrange
            var request = new UpdateRoleRequest { Name = "New Role" };
            _repository.Roles.GetById(ExistingRole.Id).Returns(ExistingRole);
            _repository.Roles
                .When(x => x.Update(Arg.Any<ApplicationRole>()))
                .Do(call =>
            {
                var role = call.Arg<ApplicationRole>();
                ExistingRole.Name = role.Name;
            });

            //Act
            await _service.Update(request, ExistingRole.Id);

            //Assert
            Assert.Equal(request.Name, ExistingRole.Name);
            _repository.Roles.Received(1).Update(Arg.Is<ApplicationRole>(
                x => x.Name == request.Name));
            await _repository.Received(1).SaveAsync();
        }

        [Fact]
        public async Task Update_ShouldThrowNotFoundException_WhenRoleDoesntExist()
        {
            //Arrange
            var request = new UpdateRoleRequest { Name = "New Role" };
            _repository.Roles.GetById(ExistingRole.Id).Returns(Task.FromResult<ApplicationRole>(null));

            //Act
            var exception = await Assert.ThrowsAsync<NotFoundException>(
                async () => await _service.Update(request, ExistingRole.Id));

            //Assert
            Assert.NotNull(exception);
            Assert.IsType<NotFoundException>(exception);
            Assert.Contains(ExistingRole.Id.ToString(), exception.ErrorMessage);
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllRolesAsRoleResponse_WhenRolesExist()
        {
            //Arrange
            _repository.Roles.GetAll().Returns(AllRoles);

            //Act
            var rolesResponse = await _service.GetAll();

            //Assert
            Assert.NotNull(rolesResponse);
            Assert.IsType<List<RoleResponse>>(rolesResponse);
            Assert.All(rolesResponse, role => Assert.NotNull(role.Name));
        }

        private ApplicationRole ExistingRole { get; }
        private IEnumerable<ApplicationRole> AllRoles;
    }
}
