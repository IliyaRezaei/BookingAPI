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

namespace Booking.Application.Tests.Unit.Mappers
{
    public class CityMapperTests
    {
        public Country Country { get; set; } = new Country
        {
            Id = Guid.NewGuid(),
            Name = "Country",
            NormalizedName = "COUNTRY",
            ImageUrl = ""
        };

        [Fact]
        public void ToEntity_ShouldReturnValidCityEntity_FromCreateCityRequest()
        {
            //Arrange
            var request = new CreateCityRequest { Name = "Cooler" };

            //Act
            var entity = request.ToEntity(Country);

            //Assert
            Assert.IsType<Guid>(entity.Id);
            Assert.NotEqual(Guid.Empty, entity.Id);
            Assert.Equal(request.Name, entity.Name);
            Assert.Equal(request.Name.ToUpper(), entity.NormalizedName);
            Assert.NotNull(entity.Country);
            Assert.IsType<Country>(entity.Country);
        }

        [Fact]
        public void ToEntity_ShouldReturnValidCityEntity_FromUpdateCityRequest()
        {
            //Arrange
            var request = new UpdateCityRequest { Name = "Cooler" };

            //Act
            var entity = request.ToEntity(Guid.NewGuid(), Country);

            //Assert
            Assert.IsType<Guid>(entity.Id);
            Assert.NotEqual(Guid.Empty, entity.Id);
            Assert.Equal(request.Name, entity.Name);
            Assert.Equal(request.Name.ToUpper(), entity.NormalizedName);
            Assert.NotNull(entity.Country);
            Assert.IsType<Country>(entity.Country);
        }
        [Fact]
        public void ToResponse_ShouldReturnValidCityResponse_FromAnCityEntity()
        {
            //Arrange
            var entity = new City
            {
                Id = Guid.NewGuid(),
                Name = "City",
                NormalizedName = "City",
                ImageUrl = "",
                Country = Country
            };

            //Act
            var response = entity.ToResponse();

            //Assert
            Assert.NotNull(response);
            Assert.IsType<CityResponse>(response);
            Assert.Equal(entity.Id, response.Id);
            Assert.Equal(entity.Name, response.Name);
            Assert.Equal(entity.ImageUrl, response.ImageUrl);
        }

        [Fact]
        public void ToResponse_ShouldReturnValidCitiesResponses_FromListOfCityEntities()
        {
            //Arrange
            var entities = new List<City>
            {
                new City { Id = Guid.NewGuid(), Name = "FirstCity", NormalizedName = "FIRSTCITY", ImageUrl = "", Country = Country},
                new City { Id = Guid.NewGuid(), Name = "SecondCity", NormalizedName = "SECONDCITY", ImageUrl = "", Country = Country},
            };

            //Act
            var responses = entities.ToResponse();

            //Assert
            Assert.NotNull(responses);
            Assert.IsType<List<CityResponse>>(responses);
            Assert.All(entities, entity => Assert.Contains(responses, city => city.Name == entity.Name));
            Assert.All(entities, entity => Assert.Contains(responses, city => city.Id == entity.Id));
            Assert.All(entities, entity => Assert.Contains(responses, city => city.ImageUrl == entity.ImageUrl));
        }
    }
}
