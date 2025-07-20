using Booking.Application.Services;
using Booking.Application.Validators.Amenity;
using Booking.Domain.Abstractions.Repositories.Manager;
using Booking.Domain.Contracts.Amenity;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.Unit.Tests.Validators.Amenity
{
    public class UpdateAmenityRequestValidatorTests
    {
        private readonly IRepositoryManager _repository;
        private readonly UpdateAmenityRequestValidator _validator;
        private readonly Domain.Entities.Amenity _amenity;

        public UpdateAmenityRequestValidatorTests()
        {
            _repository = Substitute.For<IRepositoryManager>();
            _amenity = new Domain.Entities.Amenity { Id = Guid.NewGuid(), Name = "Cooler", NormalizedName = "COOLER" };
            _validator = new UpdateAmenityRequestValidator(_repository, _amenity.Id);
        }

        [Fact]
        public async Task ValidRequest_ShouldPass()
        {
            //Arrange
            var request = new UpdateAmenityRequest { Name = "Cooler" };

            //Act
            var result = await _validator.ValidateAsync(request);
            //Assert
            Assert.True(result.IsValid);
        }

        [Fact]
        public async Task NameIsNotChanged_ShouldPass()
        {
            //Arrange
            var request = new UpdateAmenityRequest { Name = "Cooler" };
            _repository.Amenities.GetByName("Cooler").Returns(_amenity);
            //Act
            var result = await _validator.ValidateAsync(request);
            //Assert
            Assert.True(result.IsValid);
        }

        [Fact]
        public async Task NameTooLong_ShouldFailAndThrowBadRequestException()
        {
            //Arrange
            var request = new UpdateAmenityRequest { Name = "Cooooooooooooooooooooooooooooooooooooooooler" };
            //Act
            var result = await _validator.ValidateAsync(request);
            //Assert
            Assert.False(result.IsValid);
        }

        [Fact]
        public async Task NotAUniqueName_ShouldFailAndThrowBadRequestException()
        {
            //Arrange
            var request = new UpdateAmenityRequest { Name = "Cooler" };
            var expectedAmenity = new Domain.Entities.Amenity
            {
                Id = Guid.NewGuid(),
                Name = "Cooler",
                NormalizedName = "COOLER"
            };
            _repository.Amenities.GetByName("Cooler").Returns(expectedAmenity);
            //Act
            var result = await _validator.ValidateAsync(request);
            //Assert
            Assert.False(result.IsValid);
        }

        [Fact]
        public async Task NullName_ShouldFailAndThrowBadRequestException()
        {
            //Arrange
            var request = new UpdateAmenityRequest { Name = null };
            //Act
            var result = await _validator.ValidateAsync(request);
            //Assert
            Assert.False(result.IsValid);
        }

        [Fact]
        public async Task EmptyName_ShouldFailAndThrowBadRequestException()
        {
            //Arrange
            var request = new UpdateAmenityRequest { Name = "" };
            //Act
            var result = await _validator.ValidateAsync(request);
            //Assert
            Assert.False(result.IsValid);
        }
    }
}
