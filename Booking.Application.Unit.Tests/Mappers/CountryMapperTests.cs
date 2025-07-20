using Booking.Application.Mappers;
using Booking.Domain.Contracts.Amenity;
using Booking.Domain.Contracts.Country;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.Unit.Tests.Mappers
{
    public class CountryMapperTests
    {
        [Fact]
        public void CreateCountryRequestMapper_ShouldMakeAValidId()
        {
            //Arrange
            var request = new CreateCountryRequest { Name = "Cooler" };

            //Act
            var entity = request.ToEntity();

            //Assert
            Assert.IsType<Guid>(entity.Id);
            Assert.NotEqual(Guid.Empty, entity.Id);
            Assert.Equal(request.Name, entity.Name);
            Assert.Equal(request.Name.ToUpper(), entity.NormalizedName);
        }

        [Fact]
        public void UpdateCountryRequestMapper_ShouldMapValid()
        {
            //Arrange
            var request = new UpdateCountryRequest { Name = "Cooler" };

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
