using Booking.Application.Mappers;
using Booking.Application.Services;
using Booking.Domain.Abstractions.Repositories.Manager;
using Booking.Domain.Contracts.Amenity;
using Booking.Domain.Contracts.Country;
using Booking.Domain.Entities;
using Booking.Domain.Errors;
using FluentValidation;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.Tests.Unit.Services
{
    public class CountryServiceTests
    {
        private readonly IRepositoryManager _repository;
        private readonly CountryService _service;

        public CountryServiceTests()
        {
            _repository = Substitute.For<IRepositoryManager>();
            _service = new CountryService(_repository);
            ExistingCountry = new Country { Id = Guid.NewGuid(), Name = "Country", NormalizedName = "COUNTRY", ImageUrl = "" };
            AllCountries = new List<Country>
            {
                new Country { Id = Guid.NewGuid(), Name = "FirstCountry", NormalizedName = "FIRSTCOUNTRY", ImageUrl = "" },
                new Country { Id = Guid.NewGuid(), Name = "SecondCountry", NormalizedName = "SECONDCOUNTRY", ImageUrl = "" },
                new Country { Id = Guid.NewGuid(), Name = "ThirdCountry", NormalizedName = "THIRDCOUNTRY", ImageUrl = "" },
            };
        }

        [Fact]
        public async Task Create_ShouldCreateAndReturnCountryResponse_WhenValidRequest()
        {
            //Arrange
            var request = new CreateCountryRequest { Name = "Iran" };

            //Act
            var result = await _service.Create(request);

            //Assert
            await _repository.Countries.Received(1).Create(
                Arg.Is<Country>(
                    e => e.Name == "Iran" &&
                    e.NormalizedName == "IRAN")
                );
            await _repository.Countries.Received(1).GetByName(request.Name);
            await _repository.Received(1).SaveAsync();

            Assert.NotNull(result);
            Assert.IsType<CountryResponse>(result);
            Assert.IsType<Guid>(result.Id);
            Assert.NotEqual(result.Id, Guid.Empty);
            Assert.Equal(result.Name, request.Name);
            Assert.Null(result.ImageUrl);
        }

        [Fact]
        public async Task Create_ShouldThrowValidationException_WhenNameIsNotUnique()
        {
            //Arrange
            var request = new CreateCountryRequest { Name = "Country" };
            _repository.Countries.GetByName(ExistingCountry.Name).Returns(ExistingCountry);

            //Act
            var exception = await Assert.ThrowsAsync<ValidationException>(async () => await _service.Create(request));

            //Assert
            await _repository.Countries.Received(1).GetByName(request.Name);

            Assert.NotNull(exception);
            Assert.IsType<ValidationException>(exception);
        }

        [Fact]
        public async Task Update_ShouldUpdate_WhenCountryExists()
        {
            //Arrange
            var request = new UpdateCountryRequest
            {
                Name = "New Cooler",
            };
            _repository.Countries.GetById(ExistingCountry.Id).Returns(ExistingCountry);
            _repository.Countries
                .When(x => x.Update(Arg.Any<Country>()))
                .Do(call =>
                {
                    var country = call.Arg<Country>();
                    ExistingCountry.Name = country.Name;
                });

            //Act
            await _service.Update(request, ExistingCountry.Id);

            //Assert
            _repository.Countries.Received(1).Update(
                Arg.Is<Country>(
                    e => e.Id == ExistingCountry.Id &&
                    e.Name == request.Name &&
                    e.NormalizedName == request.Name.ToUpper())
                );
            await _repository.Countries.Received(1).GetById(ExistingCountry.Id);
            await _repository.Received(1).SaveAsync();
            Assert.Equal(request.Name, ExistingCountry.Name);
        }

        [Fact]
        public async Task Update_ShouldThrowNotFoundException_WhenCountryDoesntExist()
        {
            //Arrange
            var id = Guid.NewGuid();
            var request = new UpdateCountryRequest { Name = "NewCooler" };
            _repository.Countries.GetById(id).Returns(Task.FromResult<Country>(null));

            //Act
            var exception = await Assert.ThrowsAsync<NotFoundException>(() => _service.Update(request, id));

            //Assert
            await _repository.Countries.Received(1).GetById(id);

            Assert.NotNull(exception);
            Assert.Contains(id.ToString(), exception.ErrorMessage);
        }

        [Fact]
        public async Task Delete_ShouldDelete_WhenCountryExists()
        {
            //Arrange
            _repository.Countries.GetById(ExistingCountry.Id).Returns(ExistingCountry);

            // Act
            await _service.Delete(ExistingCountry.Id);

            // Assert
            _repository.Countries.Received(1).Delete(
                Arg.Is<Country>(
                    e => e.Id == ExistingCountry.Id &&
                    e.Name == ExistingCountry.Name &&
                    e.NormalizedName == ExistingCountry.NormalizedName)
                );
            await _repository.Countries.Received(1).GetById(ExistingCountry.Id);
            await _repository.Received(1).SaveAsync();
        }

        [Fact]
        public async Task Delete_ShouldThrowNotFoundException_WhenCountryDoesntExist()
        {
            //Arrange
            var id = Guid.NewGuid();
            _repository.Countries.GetById(id).Returns(Task.FromResult<Country>(null));

            //Act
            var exception = await Assert.ThrowsAsync<NotFoundException>(async () => await _service.Delete(id));

            //Assert
            await _repository.Countries.Received(1).GetById(id);

            Assert.NotNull(exception);
            Assert.Contains(id.ToString(), exception.ErrorMessage);
        }

        [Fact]
        public async Task GetById_ShouldReturnCountryResponse_WhenCountryExists()
        {
            //Arrange
            _repository.Countries.GetById(ExistingCountry.Id).Returns(ExistingCountry);

            //Act
            var result = await _service.GetById(ExistingCountry.Id);

            //Assert
            await _repository.Countries.Received(1).GetById(ExistingCountry.Id);

            Assert.NotNull(result);
            Assert.IsType<CountryResponse>(result);
            Assert.Equal(ExistingCountry.Id, result.Id);
            Assert.Equal(ExistingCountry.Name, result.Name);
        }

        [Fact]
        public async Task GetById_ShouldThrowNotFoundException_WhenCountryDoesntExist()
        {
            //Arrange
            var id = Guid.NewGuid();
            _repository.Countries.GetById(id).Returns(Task.FromResult<Country>(null));

            //Act
            var exception = await Assert.ThrowsAsync<NotFoundException>(async () => await _service.GetById(id));

            //Assert
            await _repository.Countries.Received(1).GetById(id);

            Assert.NotNull(exception);
            Assert.Contains(id.ToString(), exception.Message);
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllCountriesAsCountryResponse_EvenIfCountriesDontExist()
        {
            //Arrange
            _repository.Countries.GetAll().Returns(AllCountries);

            //Act
            var countriesResponse = await _service.GetAll();

            //Assert
            Assert.NotNull(countriesResponse);
            Assert.IsType<List<CountryResponse>>(countriesResponse);
            Assert.Equal(AllCountries.Count(), countriesResponse.Count());
            Assert.All(countriesResponse, country => Assert.NotNull(country.Name));
        }
        private Country ExistingCountry { get; }
        private IEnumerable<Country> AllCountries;
    }
}
