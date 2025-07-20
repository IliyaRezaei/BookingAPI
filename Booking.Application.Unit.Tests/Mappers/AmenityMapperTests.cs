using Booking.Application.Mappers;
using Booking.Domain.Contracts.Amenity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.Unit.Tests.Mappers
{
    public class AmenityMapperTests
    {
        [Fact]
        public void CreateAmenityRequestMapper_ShouldMakeAValidId()
        {
            //Arrange
            var request = new CreateAmenityRequest { Name = "Cooler" };

            //Act
            var entity = request.ToEntity();

            //Assert
            Assert.IsType<Guid>(entity.Id);
            Assert.NotEqual(Guid.Empty, entity.Id);
            Assert.Equal(request.Name, entity.Name);
            Assert.Equal(request.Name.ToUpper(), entity.NormalizedName);
        }

        [Fact]
        public void UpdateAmenityRequestMapper_ShouldMapValid()
        {
            //Arrange
            var request = new UpdateAmenityRequest { Name = "Cooler" };

            //Act
            var entity = request.ToEntity(Guid.NewGuid());

            //Assert
            Assert.IsType<Guid>(entity.Id);
            Assert.NotEqual(Guid.Empty, entity.Id);
            Assert.Equal(request.Name, entity.Name);
            Assert.Equal(request.Name.ToUpper(), entity.NormalizedName);
        }
    }
}
