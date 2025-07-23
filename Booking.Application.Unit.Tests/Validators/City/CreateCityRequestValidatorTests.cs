using Booking.Application.Validators.City;
using Booking.Domain.Abstractions.Repositories.Manager;
using Booking.Domain.Contracts.City;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.Tests.Unit.Validators.City
{
    public class CreateCityRequestValidatorTests
    {
        private readonly IRepositoryManager _repository;
        private readonly CreateCityRequestValidator _validator;
        public CreateCityRequestValidatorTests()
        {
            _repository = Substitute.For<IRepositoryManager>();
            _validator = new CreateCityRequestValidator(_repository);
        }

        [Theory]
        [InlineData("City")]
        [InlineData("Iran")]
        [InlineData("City 20 Characters")]
        public async Task ShouldPass_WhenNameIsValid(string name)
        {
            //Arrange
            var request = new CreateCityRequest { Name = name };
            //Act
            var result = await _validator.ValidateAsync(request);
            //Assert
            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("Ciiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiity")]
        [InlineData("City")]
        public async Task ShouldFail_WhenNameIsInvalidOrNotUnique(string name)
        {
            //Arrange
            var request = new CreateCityRequest { Name = name };
            var existingCity = new Domain.Entities.City
            {
                Id = Guid.NewGuid(),
                Name = "City",
                NormalizedName = "CITY",
                Country = new Domain.Entities.Country { Id = Guid.NewGuid(), Name = "Country", NormalizedName = "COUNTRY" },
                ImageUrl = ""
            };
            _repository.Cities.GetByName(existingCity.Name).Returns(existingCity);
            //Act
            var result = await _validator.ValidateAsync(request);
            //Assert
            Assert.False(result.IsValid);
        }
    }
}
