using Booking.Application.Validators.City;
using Booking.Domain.Abstractions.Repositories.Manager;
using Booking.Domain.Contracts.City;
using FluentValidation;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.Tests.Unit.Validators.City
{
    public class UpdateCityRequestValidatorTests
    {
        private readonly IRepositoryManager _repository;
        private readonly UpdateCityRequestValidator _validator;
        private readonly Domain.Entities.City _city;
        public UpdateCityRequestValidatorTests()
        {
            _repository = Substitute.For<IRepositoryManager>();
            _city = new Domain.Entities.City
            {
                Id = Guid.NewGuid(),
                Name = "City",
                NormalizedName = "CITY",
                Country = new Domain.Entities.Country { Id = Guid.NewGuid(), Name = "Country", NormalizedName = "COUNTRY" },
                ImageUrl = ""
            };
            _validator = new UpdateCityRequestValidator(_repository, _city.Id);
        }

        [Theory]
        [InlineData("City")]
        [InlineData("Iran")]
        [InlineData("City 20 Characters")]
        public async Task ShouldPass_WhenNameIsValid(string name)
        {
            //Arrange
            var request = new UpdateCityRequest { Name = name, CountryId = _city.Country.Id };
            _repository.Cities.GetByName(_city.Name).Returns(_city);
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
            var request = new UpdateCityRequest { Name = name, CountryId = _city.Country.Id };
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
