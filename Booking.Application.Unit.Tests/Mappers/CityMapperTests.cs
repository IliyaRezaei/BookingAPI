using Booking.Application.Mappers;
using Booking.Domain.Contracts.Amenity;
using Booking.Domain.Contracts.City;
using Booking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.Unit.Tests.Mappers
{
    public class CityMapperTests
    {
        [Fact]
        public void CreateCityRequestMapper_ShouldMakeAValidIdAndCountry()
        {
            //Arrange
            var request = new CreateCityRequest { Name = "Cooler" };
            var country = new Country 
            { 
                Id = Guid.NewGuid(),
                Name = "Country",
                NormalizedName = "COUNTRY",
                ImageUrl = ""
            };

            //Act
            var entity = request.ToEntity(country);

            //Assert
            Assert.IsType<Guid>(entity.Id);
            Assert.NotEqual(Guid.Empty, entity.Id);
            Assert.Equal(request.Name, entity.Name);
            Assert.Equal(request.Name.ToUpper(), entity.NormalizedName);
            Assert.NotNull(entity.Country);
            Assert.IsType<Country>(entity.Country);
        }

        [Fact]
        public void UpdateCityRequestMapper_ShouldMapValidAndCountry()
        {
            //Arrange
            var request = new UpdateCityRequest { Name = "Cooler" };
            var country = new Country
            {
                Id = Guid.NewGuid(),
                Name = "Country",
                NormalizedName = "COUNTRY",
                ImageUrl = ""
            };

            //Act
            var entity = request.ToEntity(Guid.NewGuid(), country);

            //Assert
            Assert.IsType<Guid>(entity.Id);
            Assert.NotEqual(Guid.Empty, entity.Id);
            Assert.Equal(request.Name, entity.Name);
            Assert.Equal(request.Name.ToUpper(), entity.NormalizedName);
            Assert.NotNull(entity.Country);
            Assert.IsType<Country>(entity.Country);
        }
    }
}
