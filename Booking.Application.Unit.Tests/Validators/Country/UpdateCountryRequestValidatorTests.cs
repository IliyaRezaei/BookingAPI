using Booking.Application.Validators.Country;
using Booking.Domain.Abstractions.Repositories.Manager;
using Booking.Domain.Contracts.Country;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.Tests.Unit.Validators.Country
{
    public class UpdateCountryRequestValidatorTests
    {
        private readonly IRepositoryManager _repository;
        private readonly UpdateCountryRequestValidator _validator;
        private readonly Domain.Entities.Country _country;
        public UpdateCountryRequestValidatorTests()
        {
            _repository = Substitute.For<IRepositoryManager>();
            _country = new Domain.Entities.Country
            {
                Id = Guid.NewGuid(),
                Name = "Country",
                NormalizedName = "COUNTRY",
                ImageUrl = ""
            };
            _validator = new UpdateCountryRequestValidator(_repository, _country.Id);
        }

        [Theory]
        [InlineData("Country")]
        [InlineData("Country 20 Chars")]
        public async Task ShouldPass_WhenNameIsValid(string name)
        {
            //Arrange
            var request = new UpdateCountryRequest { Name = name };
            _repository.Countries.GetById(_country.Id).Returns(_country);
            //Act
            var result = await _validator.ValidateAsync(request);
            //Assert
            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("Country")]
        [InlineData("Countryyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyy")]
        public async Task ShouldFail_WhenNameIsInvalidOrNotUnique(string name)
        {
            //Arrange
            var request = new UpdateCountryRequest { Name = name };
            var expectedCountry = new Domain.Entities.Country
            {
                Id = Guid.NewGuid(),
                Name = "Country",
                NormalizedName = "COUNTRY",
                ImageUrl = ""
            };
            _repository.Countries.GetByName(expectedCountry.Name).Returns(expectedCountry);
            //Act
            var result = await _validator.ValidateAsync(request);
            //Assert
            Assert.False(result.IsValid);
        }
    }
}
