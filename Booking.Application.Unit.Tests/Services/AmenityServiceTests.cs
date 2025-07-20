using Booking.Application.Mappers;
using Booking.Application.Services;
using Booking.Domain.Abstractions.Repositories.Manager;
using Booking.Domain.Contracts.Amenity;
using Booking.Domain.Entities;
using Booking.Domain.Errors;
using FluentValidation;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.Unit.Tests.Services
{
    public class AmenityServiceTests
    {
        private readonly IRepositoryManager _repository;
        private readonly AmenityService _service;

        public AmenityServiceTests()
        {
            _repository = Substitute.For<IRepositoryManager>();
            _service = new AmenityService(_repository);
        }

        [Fact]
        public async Task Create_NonExistingAmenity_ShouldCreateAndSaveChangesThenReturnAmenityResponse()
        {
            //Arrange
            var request = new CreateAmenityRequest { Name = "Cooler" };
            var entity = request.ToEntity();

            //Act
            var result = await _service.Create(request);

            //Assert
            await _repository.Amenities.Received(1).Create(
                Arg.Is<Amenity>(
                    e => e.Name == "Cooler" &&
                    e.NormalizedName == "COOLER")
                );
            await _repository.Amenities.Received(1).GetByName(request.Name);
            await _repository.Received(1).SaveAsync();

            Assert.NotNull(result);
            Assert.IsType<AmenityResponse>(result);
            Assert.IsType<Guid>(result.Id);
            Assert.NotEqual(result.Id, Guid.Empty);
            Assert.Equal(result.Name, entity.Name);
            Assert.Null(result.ImageUrl);
        }

        [Fact]
        public async Task Create_ExistingAmenity_ShouldFailAndThrowValidationException()
        {
            //Arrange
            var request = new CreateAmenityRequest { Name = "Cooler" };
            var entity = request.ToEntity();
            var existingAmenity = new Amenity
            {
                Id = Guid.NewGuid(),
                Name = "Cooler",
                NormalizedName = "COOLER",
                ImageUrl = ""
            };
            _repository.Amenities.GetByName("Cooler").Returns(existingAmenity);

            //Act
            var exception = await Assert.ThrowsAsync<ValidationException>(async () => await _service.Create(request));

            //Assert
            await _repository.Amenities.Received(1).GetByName(request.Name);

            Assert.NotNull(exception);
            Assert.IsType<ValidationException>(exception);
        }

        [Fact]
        public async Task Update_ExistingAmenity_ShouldUpdateAndSaveChangesThenReturnAmenityResponse()
        {
            //Arrange
            var id = Guid.NewGuid();
            var request = new UpdateAmenityRequest
            {
                Name = "New Cooler",
            };
            var existingEntity = new Amenity 
            { 
                Id = id, 
                Name = "Cooler",
                NormalizedName = "Cooler", 
                ImageUrl = "" 
            };
            _repository.Amenities.GetById(id).Returns(existingEntity);

            //Act
            await _service.Update(request, id);

            //Assert
            _repository.Amenities.Received(1).Update(
                Arg.Is<Amenity>(
                    e => e.Id == id &&
                    e.Name == request.Name &&
                    e.NormalizedName == request.Name.ToUpper())
                );
            await _repository.Amenities.Received(1).GetById(id);
            await _repository.Received(1).SaveAsync();
        }

        [Fact]
        public async Task Update_NonExistingAmenity_ShouldUpdateAndSaveChangesThenReturnAmenityResponse()
        {
            //Arrange
            var id = Guid.NewGuid();
            var request = new UpdateAmenityRequest { Name = "NewCooler" };
            _repository.Amenities.GetById(id).Returns(Task.FromResult<Amenity>(null));

            //Act
            var exception = await Assert.ThrowsAsync<NotFoundException>(() => _service.Update(request, id));

            //Assert
            await _repository.Amenities.Received(1).GetById(id);

            Assert.NotNull(exception);
            Assert.Contains(id.ToString(), exception.ErrorMessage);
        }

        [Fact]
        public async Task Delete_ExistingAmenity_ShouldDeleteAndSaveChanges()
        {
            //Arrange
            var id = Guid.NewGuid();
            var existingAmenity = new Amenity 
            { 
                Id = id, 
                Name = "Cooler", 
                NormalizedName = "COOLER" 
            };
            _repository.Amenities.GetById(id).Returns(existingAmenity);

            // Act
            await _service.Delete(id);

            // Assert
            _repository.Amenities.Received(1).Delete(
                Arg.Is<Amenity>(
                    e => e.Id == existingAmenity.Id &&
                    e.Name == "Cooler" &&
                    e.NormalizedName == "COOLER")
                );
            await _repository.Amenities.Received(1).GetById(id);
            await _repository.Received(1).SaveAsync();
        }

        [Fact]
        public async Task Delete_NonExistingAmenity_ShouldThrowsNotFoundException()
        {
            //Arrange
            var id = Guid.NewGuid();
            _repository.Amenities.GetById(id).Returns(Task.FromResult<Amenity>(null));

            //Act
            var exception = await Assert.ThrowsAsync<NotFoundException>(async () => await _service.Delete(id));

            //Assert
            await _repository.Amenities.Received(1).GetById(id);

            Assert.NotNull(exception);
            Assert.Contains(id.ToString(), exception.ErrorMessage);
        }

        [Fact]
        public async Task GetById_ExistingAmenity_ShouldReturnAmenityResponse()
        {
            //Arrange
            var id = Guid.NewGuid();

            var expectedAmenity = new Amenity
            {
                Id = id,
                Name = "Cooler",
                NormalizedName = "COOLER",
                ImageUrl = ""
            };

            _repository.Amenities.GetById(id).Returns(expectedAmenity);

            //Act
            var result = await _service.GetById(id);

            //Assert
            await _repository.Amenities.Received(1).GetById(id);

            Assert.NotNull(result);
            Assert.IsType<AmenityResponse>(result);
            Assert.Equal(expectedAmenity.Id, result.Id);
            Assert.Equal(expectedAmenity.Name, result.Name);
        }

        [Fact]
        public async Task GetById_NonExistingAmenity_ShouldThrowsNotFoundException()
        {
            //Arrange
            var id = Guid.NewGuid();
            _repository.Amenities.GetById(id).Returns(Task.FromResult<Amenity>(null));

            //Act
            var exception = await Assert.ThrowsAsync<NotFoundException>(async () => await _service.GetById(id));

            //Assert
            await _repository.Amenities.Received(1).GetById(id);

            Assert.NotNull(exception);
            Assert.Contains(id.ToString(), exception.Message);
        }

        [Fact]
        public async Task GetAll_ExistingAmenities_ShouldReturnAllAmenities()
        {
            //Arrange
            var expectedAmenities = new List<Amenity>
            {
                new Amenity 
                { 
                    Id = Guid.NewGuid(), 
                    Name = "Cooler", 
                    NormalizedName = "Cooler", 
                    ImageUrl = ""
                },new Amenity
                {
                    Id = Guid.NewGuid(),
                    Name = "Heater",
                    NormalizedName = "HEATER",
                    ImageUrl = ""
                },new Amenity
                {
                    Id = Guid.NewGuid(),
                    Name = "Gym",
                    NormalizedName = "GYM",
                    ImageUrl = ""
                },
            };

            _repository.Amenities.GetAll().Returns(expectedAmenities);


            //Act
            var result = await _service.GetAll();

            //Assert
            await _repository.Amenities.Received(1).GetAll();

            Assert.NotNull(result);
            Assert.IsType<List<AmenityResponse>>(result);
            int count = 0;
            foreach (var amenity in result)
            {
                var expected = expectedAmenities[count++];
                Assert.IsType<Guid>(amenity.Id);
                Assert.NotEqual(Guid.Empty, amenity.Id);
                Assert.Equal(expected.Id, amenity.Id);
                Assert.Equal(expected.Name, amenity.Name);
            }
            Assert.Equal(expectedAmenities.Count, count);
        }
    }
}
