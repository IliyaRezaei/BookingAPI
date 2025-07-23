using Booking.Application.Mappers;
using Booking.Application.Services;
using Booking.Domain.Abstractions.Repositories.Manager;
using Booking.Domain.Contracts.Amenity;
using Booking.Domain.Contracts.City;
using Booking.Domain.Contracts.Country;
using Booking.Domain.Entities;
using Booking.Domain.Errors;
using FluentValidation;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Booking.Application.Tests.Unit.Services
{
    public class CityServiceTests
    {
        private readonly IRepositoryManager _repository;
        private readonly CityService _service;

        public CityServiceTests()
        {
            _repository = Substitute.For<IRepositoryManager>();
            _service = new CityService(_repository);
            ExistingCountry = new Country { Id = Guid.NewGuid(), Name = "Country", NormalizedName = "COUNTRY", ImageUrl = "" };
            ExistingCity = new City { Id = Guid.NewGuid(), Name = "City", NormalizedName = "CITY", Country = ExistingCountry, ImageUrl = "" };
            AllCities = new List<City>
            {
                new City { Id = Guid.NewGuid(), Name = "FirstCity", NormalizedName = "FIRSTCITY" , Country = ExistingCountry},
                new City { Id = Guid.NewGuid(), Name = "SecondCity", NormalizedName = "SECONDCITY", Country = ExistingCountry},
                new City { Id = Guid.NewGuid(), Name = "ThirdCity", NormalizedName = "THIRDCITY", Country = ExistingCountry },
            };
        }

        [Fact]
        public async Task Create_ShouldCreateAndReturnCityResponse_WhenRequestIsValid()
        {
            //Arrange
            var request = new CreateCityRequest { Name = "City" };
            _repository.Countries.GetById(ExistingCountry.Id).Returns(Task.FromResult<Country>(ExistingCountry));

            //Act
            var result = await _service.Create(request, ExistingCountry.Id);

            //Assert
            await _repository.Cities.Received(1).GetByName(request.Name);
            await _repository.Countries.Received(1).GetById(ExistingCountry.Id);
            await _repository.Cities.Received(1).Create(
                Arg.Is<City>(
                    e => e.Name == "City" &&
                    e.NormalizedName == "CITY" &&
                    e.Country == ExistingCountry)
                );
            await _repository.Received(1).SaveAsync();

            Assert.NotNull(result);
            Assert.IsType<CityResponse>(result);
            Assert.IsType<Guid>(result.Id);
            Assert.NotEqual(result.Id, Guid.Empty);
            Assert.Equal(result.Name, request.Name);
            Assert.Null(result.ImageUrl);
        }

        [Fact]
        public async Task Create_ShouldThrowValidationException_WhenCityExists()
        {
            //Arrange
            var request = new CreateCityRequest { Name = "City" };

            _repository.Cities.GetByName(ExistingCity.Name).Returns(ExistingCity);

            //Act
            var exception = await Assert.ThrowsAsync<ValidationException>(async () => await _service.Create(request, ExistingCountry.Id));

            //Assert
            Assert.NotNull(exception);
            Assert.IsType<ValidationException>(exception);
        }

        [Fact]
        public async Task Update_ShouldUpdate_WhenCityExists()
        {
            //Arrange
            var request = new UpdateCityRequest
            {
                Name = "New City",
                CountryId = ExistingCountry.Id,
            };
            _repository.Countries.GetById(ExistingCountry.Id).Returns(ExistingCountry);
            _repository.Cities.GetById(ExistingCity.Id).Returns(ExistingCity);
            _repository.Cities
                .When(x => x.Update(Arg.Any<City>()))
                .Do(call =>
                {
                    var city = call.Arg<City>();
                    ExistingCity.Name = city.Name;
                });

            //Act
            await _service.Update(request, ExistingCity.Id);

            //Assert
            await _repository.Countries.Received(1).GetById(ExistingCountry.Id);
            await _repository.Cities.Received(1).GetById(ExistingCity.Id);
            _repository.Cities.Received(1).Update(
                Arg.Is<City>(
                    e => e.Id == ExistingCity.Id &&
                    e.Name == request.Name &&
                    e.NormalizedName == request.Name.ToUpper() &&
                    e.Country == ExistingCountry)
                );
            await _repository.Received(1).SaveAsync();
            Assert.Equal(request.Name, ExistingCity.Name);

        }

        [Fact]
        public async Task Update_ShouldThrowNotFoundException_WhenCityExistsButCountryDoesntExist()
        {
            //Arrange
            var countryId = Guid.NewGuid();
            var request = new UpdateCityRequest
            {
                Name = "New Cooler",
                CountryId = countryId,
            };

            _repository.Cities.GetById(ExistingCity.Id).Returns(ExistingCity);
            _repository.Countries.GetById(countryId).Returns(Task.FromResult<Country>(null));

            //Act
            var exception = await Assert.ThrowsAsync<NotFoundException>(async () => await _service.Update(request, ExistingCity.Id));

            //Assert
            await _repository.Cities.Received(1).GetById(ExistingCity.Id);
            await _repository.Countries.Received(1).GetById(countryId);
            Assert.NotNull(exception);
            Assert.IsType<NotFoundException>(exception);
            Assert.Contains(countryId.ToString(), exception.ErrorMessage);
        }

        [Fact]
        public async Task Update_ShouldThrowNotFoundException_WhenCityDoesntExist()
        {
            //Arrange
            var cityId = Guid.NewGuid();
            var request = new UpdateCityRequest { Name = "New City", CountryId = ExistingCountry.Id };
            _repository.Cities.GetById(cityId).Returns(Task.FromResult<City>(null));

            //Act
            var exception = await Assert.ThrowsAsync<NotFoundException>(() => _service.Update(request, cityId));

            //Assert
            await _repository.Cities.Received(1).GetById(cityId);

            Assert.NotNull(exception);
            Assert.Contains(cityId.ToString(), exception.ErrorMessage);
        }

        [Fact]
        public async Task Delete_ShouldDelete_WhenCityExists()
        {
            //Arrange
            _repository.Cities.GetById(ExistingCity.Id).Returns(ExistingCity);

            // Act
            await _service.Delete(ExistingCity.Id);

            // Assert
            await _repository.Cities.Received(1).GetById(ExistingCity.Id);
            _repository.Cities.Received(1).Delete(
                Arg.Is<City>(
                    e => e.Id == ExistingCity.Id &&
                    e.Name == "City" &&
                    e.NormalizedName == "CITY" &&
                    e.Country == ExistingCity.Country)
                );
            await _repository.Received(1).SaveAsync();
        }

        [Fact]
        public async Task Delete_ShouldThrowNotFoundException_WhenCityDoesntExist()
        {
            //Arrange
            var cityId = Guid.NewGuid();
            _repository.Cities.GetById(cityId).Returns(Task.FromResult<City>(null));

            //Act
            var exception = await Assert.ThrowsAsync<NotFoundException>(async () => await _service.Delete(cityId));

            //Assert
            await _repository.Cities.Received(1).GetById(cityId);
            Assert.NotNull(exception);
            Assert.Contains(cityId.ToString(), exception.ErrorMessage);
        }

        [Fact]
        public async Task GetById_ShouldReturnCityResponse_WhenCityExists()
        {
            //Arrange
            _repository.Cities.GetById(ExistingCity.Id).Returns(ExistingCity);

            //Act
            var result = await _service.GetById(ExistingCity.Id);

            //Assert
            await _repository.Cities.Received(1).GetById(ExistingCity.Id);

            Assert.NotNull(result);
            Assert.IsType<CityResponse>(result);
            Assert.Equal(ExistingCity.Id, result.Id);
            Assert.Equal(ExistingCity.Name, result.Name);
        }

        [Fact]
        public async Task GetById_ShouldThrowNotFoundException_WhenCityDoesntExist()
        {
            //Arrange
            var cityId = Guid.NewGuid();
            _repository.Cities.GetById(cityId).Returns(Task.FromResult<City>(null));

            //Act
            var exception = await Assert.ThrowsAsync<NotFoundException>(async () => await _service.GetById(cityId));

            //Assert
            await _repository.Cities.Received(1).GetById(cityId);

            Assert.NotNull(exception);
            Assert.Contains(cityId.ToString(), exception.Message);
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllCitiesAsCityResponse_EvenIfCitiesDontExist()
        {
            //Arrange
            _repository.Cities.GetAll().Returns(AllCities);

            //Act
            var citiesResponse = await _service.GetAll();

            //Assert
            await _repository.Cities.Received(1).GetAll();

            Assert.NotNull(citiesResponse);
            Assert.IsType<List<CityResponse>>(citiesResponse);
            Assert.Equal(AllCities.Count(), citiesResponse.Count());
            Assert.All(citiesResponse, city => Assert.NotNull(city.Name));
        }

        private Country ExistingCountry { get; }
        private City ExistingCity { get; }
        private IEnumerable<City> AllCities;
    }
}
