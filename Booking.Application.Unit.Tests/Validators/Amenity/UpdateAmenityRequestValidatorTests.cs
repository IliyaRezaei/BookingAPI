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

        [Theory]
        [InlineData("Heater")]
        [InlineData("Cooler")]
        [InlineData("Amenity 20 Character")]
        public async Task ShouldPass_WhenValidName(string name)
        {
            //Arrange
            var request = new UpdateAmenityRequest { Name = name };
            _repository.Amenities.GetByName(_amenity.Name).Returns(_amenity);
            //Act
            var result = await _validator.ValidateAsync(request);
            //Assert
            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("Cooooooooooooooooooooooooooooooooooooooooooooler")]
        [InlineData("Heater")]
        public async Task ShouldFail_WhenNameIsInvalidOrNotUnique(string name)
        {
            //Arrange
            var request = new UpdateAmenityRequest { Name = name };
            var expectedAmenity = new Domain.Entities.Amenity
            {
                Id = Guid.NewGuid(),
                Name = "Heater",
                NormalizedName = "HEATER"
            };
            _repository.Amenities.GetByName("Heater").Returns(expectedAmenity);
            //Act
            var result = await _validator.ValidateAsync(request);
            //Assert
            Assert.False(result.IsValid);
        }
    }
}
