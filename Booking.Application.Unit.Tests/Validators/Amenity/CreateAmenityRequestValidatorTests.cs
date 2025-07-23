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

namespace Booking.Application.Tests.Unit.Validators.Amenity
{
    public class CreateAmenityRequestValidatorTests
    {
        private readonly IRepositoryManager _repository;
        private readonly CreateAmenityRequestValidator _validator;

        public CreateAmenityRequestValidatorTests()
        {
            _repository = Substitute.For<IRepositoryManager>();
            _validator = new CreateAmenityRequestValidator(_repository);
        }

        [Fact]
        public async Task ValidRequest_ShouldPass()
        {
            //Arrange
            var request = new CreateAmenityRequest { Name = "Cooler" };
            //Act
            var result = await _validator.ValidateAsync(request);
            //Assert
            Assert.True(result.IsValid);
        }

        [Fact]
        public async Task NameTooLong_ShouldFailAndThrowBadRequestException()
        {
            //Arrange
            var request = new CreateAmenityRequest { Name = "Cooooooooooooooooooooooooooooooooooooooooler" };
            //Act
            var result = await _validator.ValidateAsync(request);
            //Assert
            Assert.False(result.IsValid);
        }

        [Fact]
        public async Task NotAUniqueName_ShouldFailAndThrowBadRequestException()
        {
            //Arrange
            var request = new CreateAmenityRequest { Name = "Cooler" };
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
            var request = new CreateAmenityRequest { Name = null };
            //Act
            var result = await _validator.ValidateAsync(request);
            //Assert
            Assert.False(result.IsValid);
        }

        [Fact]
        public async Task EmptyName_ShouldFailAndThrowBadRequestException()
        {
            //Arrange
            var request = new CreateAmenityRequest { Name = "" };
            //Act
            var result = await _validator.ValidateAsync(request);
            //Assert
            Assert.False(result.IsValid);
        }
    }
}
