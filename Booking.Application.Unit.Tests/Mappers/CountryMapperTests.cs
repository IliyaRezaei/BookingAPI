using Booking.Application.Mappers;
using Booking.Domain.Contracts.Amenity;
using Booking.Domain.Contracts.Country;
using Booking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.Tests.Unit.Mappers
{
    public class CountryMapperTests
    {
        [Fact]
        public void ToEntity_ShouldReturnValidCountryEntity_FromCreateCountryRequest()
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
        public void ToEntity_ShouldReturnValidCountryEntity_FromUpdateCountryRequest()
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
        [Fact]
        public void ToResponse_ShouldReturnValidCountryResponse_FromAnCountryEntity()
        {
            //Arrange
            var entity = new Country
            {
                Id = Guid.NewGuid(),
                Name = "Country",
                NormalizedName = "COUNTRY",
                ImageUrl = ""
            };

            //Act
            var response = entity.ToResponse();

            //Assert
            Assert.NotNull(response);
            Assert.IsType<CountryResponse>(response);
            Assert.Equal(entity.Id, response.Id);
            Assert.Equal(entity.Name, response.Name);
            Assert.Equal(entity.ImageUrl, response.ImageUrl);
        }

        [Fact]
        public void ToResponse_ShouldReturnValidCountryResponses_FromListOfCountryEntities()
        {
            //Arrange
            var entities = new List<Country>
            {
                new Country { Id = Guid.NewGuid(), Name = "FirstCountry", NormalizedName = "FIRSTCOUNTRY", ImageUrl = ""},
                new Country { Id = Guid.NewGuid(), Name = "SecondCountry", NormalizedName = "SECONDCOUNTRY", ImageUrl = ""},
            };

            //Act
            var responses = entities.ToResponse();

            //Assert
            Assert.NotNull(responses);
            Assert.IsType<List<CountryResponse>>(responses);
            Assert.All(entities, entity => Assert.Contains(responses, country => country.Name == entity.Name));
            Assert.All(entities, entity => Assert.Contains(responses, country => country.Id == entity.Id));
            Assert.All(entities, entity => Assert.Contains(responses, country => country.ImageUrl == entity.ImageUrl));
        }
    }
}
