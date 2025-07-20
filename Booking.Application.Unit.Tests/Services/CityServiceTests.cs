using Booking.Application.Mappers;
using Booking.Application.Services;
using Booking.Domain.Abstractions.Repositories.Manager;
using Booking.Domain.Contracts.Amenity;
using Booking.Domain.Contracts.City;
using Booking.Domain.Entities;
using Booking.Domain.Errors;
using FluentValidation;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.Unit.Tests.Services
{
    public class CityServiceTests
    {
        private readonly IRepositoryManager _repository;
        private readonly CityService _service;
        private readonly Country _country;

        public CityServiceTests()
        {
            _repository = Substitute.For<IRepositoryManager>();
            _service = new CityService(_repository);
            _country = new Country
            {
                Id = Guid.NewGuid(),
                Name = "Name",
                NormalizedName = "NAME",
                ImageUrl = ""
            };
        }

        [Fact]
        public async Task Create_ValidRequest_ShouldCreateAndSaveChangesThenReturnCityResponse()
        {
            //Arrange
            var request = new CreateCityRequest { Name = "City" };
            var entity = request.ToEntity(_country);
            _repository.Countries.GetById(_country.Id).Returns(Task.FromResult<Country>(_country));

            //Act
            var result = await _service.Create(request, _country.Id);

            //Assert
            await _repository.Cities.Received(1).GetByName(request.Name);
            await _repository.Countries.Received(1).GetById(_country.Id);
            await _repository.Cities.Received(1).Create(
                Arg.Is<City>(
                    e => e.Name == "City" &&
                    e.NormalizedName == "CITY" &&
                    e.Country == entity.Country)
                );
            await _repository.Received(1).SaveAsync();

            Assert.NotNull(result);
            Assert.IsType<CityResponse>(result);
            Assert.IsType<Guid>(result.Id);
            Assert.NotEqual(result.Id, Guid.Empty);
            Assert.Equal(result.Name, entity.Name);
            Assert.Null(result.ImageUrl);
        }

        [Fact]
        public async Task Create_ExistingCity_ShouldFailAndThrowValidationException()
        {
            //Arrange
            var request = new CreateCityRequest { Name = "City" };
            var entity = request.ToEntity(_country);
            var existingCity = new City
            {
                Id = Guid.NewGuid(),
                Name = "City",
                NormalizedName = "CITY",
                ImageUrl = "",
                Country = _country,
            };
            _repository.Cities.GetByName("City").Returns(existingCity);

            //Act
            var exception = await Assert.ThrowsAsync<ValidationException>(async () => await _service.Create(request, _country.Id));

            //Assert
            await _repository.Cities.Received(1).GetByName(request.Name);

            Assert.NotNull(exception);
            Assert.IsType<ValidationException>(exception);
        }

        [Fact]
        public async Task Update_ExistingCity_ShouldUpdateAndSaveChangesThenReturnCityResponse()
        {
            //Arrange
            var cityId = Guid.NewGuid();
            var request = new UpdateCityRequest
            {
                Name = "New City",
                CountryId = _country.Id,
            };
            var existingEntity = new City
            {
                Id = cityId,
                Name = "City",
                NormalizedName = "CITY",
                ImageUrl = "",
                Country = _country
            };
            _repository.Countries.GetById(_country.Id).Returns(_country);
            _repository.Cities.GetById(cityId).Returns(existingEntity);

            //Act
            await _service.Update(request, cityId);

            //Assert
            await _repository.Countries.Received(2).GetById(_country.Id);
            await _repository.Cities.Received(1).GetById(cityId);
            _repository.Cities.Received(1).Update(
                Arg.Is<City>(
                    e => e.Id == cityId &&
                    e.Name == request.Name &&
                    e.NormalizedName == request.Name.ToUpper() &&
                    e.Country == _country)
                );
            await _repository.Received(1).SaveAsync();
        }

        [Fact]
        public async Task Update_NonExistingCity_ShouldUpdateAndSaveChangesThenReturnCityResponse()
        {
            //Arrange
            var cityId = Guid.NewGuid();
            var request = new UpdateCityRequest { Name = "New City", CountryId = _country.Id };
            _repository.Countries.GetById(_country.Id).Returns(Task.FromResult<Country>(_country));
            _repository.Cities.GetById(cityId).Returns(Task.FromResult<City>(null));

            //Act
            var exception = await Assert.ThrowsAsync<NotFoundException>(() => _service.Update(request, cityId));

            //Assert
            await _repository.Countries.Received(1).GetById(_country.Id);
            await _repository.Cities.Received(1).GetById(cityId);

            Assert.NotNull(exception);
            Assert.Contains(cityId.ToString(), exception.ErrorMessage);
        }

        [Fact]
        public async Task Delete_ExistingCity_ShouldDeleteAndSaveChanges()
        {
            //Arrange
            var cityId = Guid.NewGuid();
            var existingCity = new City
            {
                Id = cityId,
                Name = "City",
                NormalizedName = "CITY",
                Country = _country,
                ImageUrl = ""
            };
            _repository.Cities.GetById(cityId).Returns(existingCity);

            // Act
            await _service.Delete(cityId);

            // Assert
            await _repository.Cities.Received(1).GetById(cityId);
            _repository.Cities.Received(1).Delete(
                Arg.Is<City>(
                    e => e.Id == existingCity.Id &&
                    e.Name == "City" &&
                    e.NormalizedName == "CITY" &&
                    e.Country == _country)
                );
            await _repository.Received(1).SaveAsync();
        }

        [Fact]
        public async Task Delete_NonExistingCity_ShouldThrowsNotFoundException()
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
        public async Task GetById_ExistingCity_ShouldReturnCityResponse()
        {
            //Arrange
            var id = Guid.NewGuid();
            var expectedCity = new City
            {
                Id = id,
                Name = "Cooler",
                NormalizedName = "COOLER",
                ImageUrl = "",
                Country = _country,
            };

            _repository.Cities.GetById(id).Returns(expectedCity);

            //Act
            var result = await _service.GetById(id);

            //Assert
            await _repository.Cities.Received(1).GetById(id);

            Assert.NotNull(result);
            Assert.IsType<CityResponse>(result);
            Assert.Equal(expectedCity.Id, result.Id);
            Assert.Equal(expectedCity.Name, result.Name);
        }

        [Fact]
        public async Task GetById_NonExistingCity_ShouldThrowsNotFoundException()
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
        public async Task GetAll_ExistingAmenities_ShouldReturnAllAmenities()
        {
            //Arrange
            var expectedCities = new List<City>
            {
                new City
                {
                    Id = Guid.NewGuid(),
                    Name = "Cooler",
                    NormalizedName = "Cooler",
                    ImageUrl = "",
                    Country = _country,
                },new City
                {
                    Id = Guid.NewGuid(),
                    Name = "Heater",
                    NormalizedName = "HEATER",
                    ImageUrl = "",
                    Country = _country
                },new City
                {
                    Id = Guid.NewGuid(),
                    Name = "Gym",
                    NormalizedName = "GYM",
                    ImageUrl = "",
                    Country = _country
                },
            };

            _repository.Cities.GetAll().Returns(expectedCities);


            //Act
            var result = await _service.GetAll();

            //Assert
            await _repository.Cities.Received(1).GetAll();

            Assert.NotNull(result);
            Assert.IsType<List<CityResponse>>(result);
            int count = 0;
            foreach (var city in result)
            {
                var expected = expectedCities[count++];
                Assert.IsType<Guid>(city.Id);
                Assert.NotEqual(Guid.Empty, city.Id);
                Assert.Equal(expected.Id, city.Id);
                Assert.Equal(expected.Name, city.Name);
            }
            Assert.Equal(expectedCities.Count, count);
        }
    }
}
