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

namespace Booking.Application.Unit.Tests.Services
{
    public class CountryServiceTests
    {
        private readonly IRepositoryManager _repository;
        private readonly CountryService _service;

        public CountryServiceTests()
        {
            _repository = Substitute.For<IRepositoryManager>();
            _service = new CountryService(_repository);
        }

        [Fact]
        public async Task Create_ValidRequest_ShouldCreateAndSaveChangesThenReturnCountryResponse()
        {
            //Arrange
            var request = new CreateCountryRequest { Name = "Iran" };
            var entity = request.ToEntity();

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
            Assert.Equal(result.Name, entity.Name);
            Assert.Null(result.ImageUrl);
        }

        [Fact]
        public async Task Create_ExistingCountry_ShouldFailAndThrowValidationException()
        {
            //Arrange
            var request = new CreateCountryRequest { Name = "Country" };
            var entity = request.ToEntity();
            var existingCountry = new Country
            {
                Id = Guid.NewGuid(),
                Name = "Country",
                NormalizedName = "COUNTRY",
                ImageUrl = ""
            };
            _repository.Countries.GetByName("Country").Returns(existingCountry);

            //Act
            var exception = await Assert.ThrowsAsync<ValidationException>(async () => await _service.Create(request));

            //Assert
            await _repository.Countries.Received(1).GetByName(request.Name);

            Assert.NotNull(exception);
            Assert.IsType<ValidationException>(exception);
        }

        [Fact]
        public async Task Update_ExistingCountry_ShouldUpdateAndSaveChangesThenReturnCountryResponse()
        {
            //Arrange
            var id = Guid.NewGuid();
            var request = new UpdateCountryRequest
            {
                Name = "New Cooler",
            };
            var existingEntity = new Country
            {
                Id = id,
                Name = "Cooler",
                NormalizedName = "Cooler",
                ImageUrl = ""
            };
            _repository.Countries.GetById(id).Returns(existingEntity);

            //Act
            await _service.Update(request, id);

            //Assert
            _repository.Countries.Received(1).Update(
                Arg.Is<Country>(
                    e => e.Id == id &&
                    e.Name == request.Name &&
                    e.NormalizedName == request.Name.ToUpper())
                );
            await _repository.Countries.Received(1).GetById(id);
            await _repository.Received(1).SaveAsync();
        }

        [Fact]
        public async Task Update_NonExistingCountry_ShouldUpdateAndSaveChangesThenReturnCountryResponse()
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
        public async Task Delete_ExistingCountry_ShouldDeleteAndSaveChanges()
        {
            //Arrange
            var id = Guid.NewGuid();
            var existingEntity = new Country
            {
                Id = id,
                Name = "Cooler",
                NormalizedName = "COOLER"
            };
            _repository.Countries.GetById(id).Returns(existingEntity);

            // Act
            await _service.Delete(id);

            // Assert
            _repository.Countries.Received(1).Delete(
                Arg.Is<Country>(
                    e => e.Id == existingEntity.Id &&
                    e.Name == "Cooler" &&
                    e.NormalizedName == "COOLER")
                );
            await _repository.Countries.Received(1).GetById(id);
            await _repository.Received(1).SaveAsync();
        }

        [Fact]
        public async Task Delete_NonExistingCountry_ShouldThrowsNotFoundException()
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
        public async Task GetById_ExistingCountry_ShouldReturnCountryResponse()
        {
            //Arrange
            var id = Guid.NewGuid();
            var expectedEntity = new Country
            {
                Id = id,
                Name = "Cooler",
                NormalizedName = "COOLER",
                ImageUrl = ""
            };
            _repository.Countries.GetById(id).Returns(expectedEntity);

            //Act
            var result = await _service.GetById(id);

            //Assert
            await _repository.Countries.Received(1).GetById(id);

            Assert.NotNull(result);
            Assert.IsType<CountryResponse>(result);
            Assert.Equal(expectedEntity.Id, result.Id);
            Assert.Equal(expectedEntity.Name, result.Name);
        }

        [Fact]
        public async Task GetById_NonExistingCountry_ShouldThrowsNotFoundException()
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
        public async Task GetAll_ExistingCountries_ShouldReturnAllCountries()
        {
            //Arrange
            var expectedEntities = new List<Country>
            {
                new Country
                {
                    Id = Guid.NewGuid(),
                    Name = "Cooler",
                    NormalizedName = "Cooler",
                    ImageUrl = ""
                },new Country
                {
                    Id = Guid.NewGuid(),
                    Name = "Heater",
                    NormalizedName = "HEATER",
                    ImageUrl = ""
                },new Country
                {
                    Id = Guid.NewGuid(),
                    Name = "Gym",
                    NormalizedName = "GYM",
                    ImageUrl = ""
                },
            };
            _repository.Countries.GetAll().Returns(expectedEntities);

            //Act
            var result = await _service.GetAll();

            //Assert
            Assert.NotNull(result);
            Assert.IsType<List<CountryResponse>>(result);
            int count = 0;
            foreach (var country in result)
            {
                var expected = expectedEntities[count++];
                Assert.IsType<Guid>(country.Id);
                Assert.NotEqual(Guid.Empty, country.Id);
                Assert.Equal(expected.Id, country.Id);
                Assert.Equal(expected.Name, country.Name);
            }
            Assert.Equal(expectedEntities.Count, count);
        }
    }
}
