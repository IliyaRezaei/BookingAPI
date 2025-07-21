using Booking.Application.Mappers;
using Booking.Domain.Contracts.Amenity;
using Booking.Domain.Contracts.Role;
using Booking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.Tests.Unit.Mappers
{
    public class RoleMapperTests
    {
        [Fact]
        public void ToEntity_ShouldReturnValidRoleEntity_FromCreateRoleRequest()
        {
            //Arrange
            var request = new CreateRoleRequest { Name = "Role" };

            //Act
            var entity = request.ToEntity();

            //Assert
            Assert.NotNull(entity);
            Assert.IsType<ApplicationRole>(entity);
            Assert.IsType<Guid>(entity.Id);
            Assert.NotNull(entity.Name);
            Assert.Equal(request.Name, entity.Name);
            Assert.NotNull(entity.NormalizedName);
            Assert.Equal(request.Name.ToUpper(), entity.NormalizedName);
        }

        [Fact]
        public void ToEntity_ShouldReturnValidRoleEntity_FromUpdateRoleRequest()
        {
            //Arrange
            var request = new UpdateRoleRequest { Name = "New Role" };

            //Act
            var entity = request.ToEntity(Guid.NewGuid());

            //Assert
            Assert.NotNull(entity);
            Assert.IsType<ApplicationRole>(entity);
            Assert.IsType<Guid>(entity.Id);
            Assert.NotNull(entity.Name);
            Assert.Equal(request.Name, entity.Name);
            Assert.NotNull(entity.NormalizedName);
            Assert.Equal(request.Name.ToUpper(), entity.NormalizedName);
        }


        [Fact]
        public void ToResponse_ShouldReturnValidRoleResponse_FromAnRoleEntity()
        {
            //Arrange
            var entity = new ApplicationRole
            {
                Id = Guid.NewGuid(),
                Name = "Role",
                NormalizedName = "ROLE",
            };

            //Act
            var response = entity.ToResponse();

            //Assert
            Assert.NotNull(response);
            Assert.IsType<RoleResponse>(response);
            Assert.Equal(entity.Id, response.Id);
            Assert.Equal(entity.Name, response.Name);
        }

        [Fact]
        public void ToResponse_ShouldReturnValidRoleResponses_FromListOfRoleEntities()
        {
            //Arrange
            var entities = new List<ApplicationRole>
            {
                new ApplicationRole { Id = Guid.NewGuid(), Name = "FirstRole", NormalizedName = "FIRSTROLE"},
                new ApplicationRole { Id = Guid.NewGuid(), Name = "SecondRole", NormalizedName = "SECONDROLE"},
            };

            //Act
            var responses = entities.ToResponse();

            //Assert
            Assert.NotNull(responses);
            Assert.IsType<List<RoleResponse>>(responses);
            Assert.All(entities, entity => Assert.Contains(responses, role => role.Name == entity.Name));
            Assert.All(entities, entity => Assert.Contains(responses, role => role.Id == entity.Id));
        }
    }
}
