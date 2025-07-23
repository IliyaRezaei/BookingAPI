using Booking.Application.Mappers;
using Booking.Domain.Contracts.Amenity;
using Booking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.Tests.Unit.Mappers
{
    public class AmenityMapperTests
    {
        [Fact]
        public void ToEntity_ShouldReturnValidAmenityEntity_FromCreateAmenityRequest()
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
        public void ToEntity_ShouldReturnValidAmenityEntity_FromUpdateAmenityRequest()
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

        [Fact]
        public void ToResponse_ShouldReturnValidAmenityResponse_FromAnAmenityEntity()
        {
            //Arrange
            var entity = new Amenity
            {
                Id = Guid.NewGuid(),
                Name = "Amenity",
                NormalizedName = "AMENITY",
                ImageUrl = ""
            };

            //Act
            var response = entity.ToResponse();

            //Assert
            Assert.NotNull(response);
            Assert.IsType<AmenityResponse>(response);
            Assert.Equal(entity.Id, response.Id);
            Assert.Equal(entity.Name, response.Name);
            Assert.Equal(entity.ImageUrl, response.ImageUrl);
        }

        [Fact]
        public void ToResponse_ShouldReturnValidAmenityResponses_FromListOfAmenityEntities()
        {
            //Arrange
            var entities = new List<Amenity>
            {
                new Amenity { Id = Guid.NewGuid(), Name = "FirstAmenity", NormalizedName = "FIRSTAMENITY", ImageUrl = ""},
                new Amenity { Id = Guid.NewGuid(), Name = "SecondAmenity", NormalizedName = "SECONDAMENITY", ImageUrl = ""},
            };

            //Act
            var responses = entities.ToResponse();

            //Assert
            Assert.NotNull(responses);
            Assert.IsType<List<AmenityResponse>>(responses);
            Assert.All(entities, entity => Assert.Contains(responses, amenity => amenity.Name == entity.Name));
            Assert.All(entities, entity => Assert.Contains(responses, amenity => amenity.Id == entity.Id));        
            Assert.All(entities, entity => Assert.Contains(responses, amenity => amenity.ImageUrl == entity.ImageUrl)); 
        }
    }
}
