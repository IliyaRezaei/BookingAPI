using Booking.Application.Mappers;
using Booking.Application.Services;
using Booking.Domain.Abstractions.Repositories.Manager;
using Booking.Domain.Contracts.Amenity;
using Booking.Domain.Contracts.Role;
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

namespace Booking.Application.Tests.Unit.Services
{
    public class AmenityServiceTests
    {
        private readonly IRepositoryManager _repository;
        private readonly AmenityService _service;

        public AmenityServiceTests()
        {
            _repository = Substitute.For<IRepositoryManager>();
            _service = new AmenityService(_repository);
            ExistingAmenity = new Amenity { Id = Guid.NewGuid(), Name = "Amenity", NormalizedName = "AMENITY" };
            AllAmenities = new List<Amenity>
            {
                new Amenity { Id = Guid.NewGuid(), Name = "FirstAmenity", NormalizedName = "FIRSTAMENITY"},
                new Amenity { Id = Guid.NewGuid(), Name = "SecondAmenity", NormalizedName = "SECONDAMENITY"},
                new Amenity { Id = Guid.NewGuid(), Name = "ThirdAmenity", NormalizedName = "THIRDAMENITY"},
            };
        }

        [Fact]
        public async Task Create_ShouldReturnAmenityResponse_WhenRequestIsValid()
        {
            //Arrange
            var request = new CreateAmenityRequest { Name = "Cooler" };

            //Act
            var result = await _service.Create(request);

            //Assert
            await _repository.Amenities.Received(1).GetByName(request.Name);
            await _repository.Amenities.Received(1).Create(
                Arg.Is<Amenity>(
                    e => e.Name == "Cooler" &&
                    e.NormalizedName == "COOLER")
                );
            await _repository.Received(1).SaveAsync();

            Assert.NotNull(result);
            Assert.IsType<AmenityResponse>(result);
            Assert.IsType<Guid>(result.Id);
            Assert.NotEqual(result.Id, Guid.Empty);
            Assert.Equal(result.Name, request.Name);
            Assert.Null(result.ImageUrl);
        }

        [Fact]
        public async Task Create_ShouldThrowValidationException_WhenNameIsNotUnique()
        {
            //Arrange
            var request = new CreateAmenityRequest { Name = ExistingAmenity.Name };
            _repository.Amenities.GetByName(ExistingAmenity.Name).Returns(ExistingAmenity);

            //Act
            var exception = await Assert.ThrowsAsync<ValidationException>(async () => await _service.Create(request));

            //Assert
            Assert.NotNull(exception);
            Assert.IsType<ValidationException>(exception);
        }

        [Fact]
        public async Task Update_ShouldUpdate_WhenAmenityExists()
        {
            //Arrange
            var request = new UpdateAmenityRequest
            {
                Name = "New Cooler",
            };
            _repository.Amenities.GetById(ExistingAmenity.Id).Returns(ExistingAmenity);
            _repository.Amenities
                .When(x => x.Update(Arg.Any<Amenity>()))
                .Do(call =>
                {
                    var amenity = call.Arg<Amenity>();
                    ExistingAmenity.Name = amenity.Name;
                });

            //Act
            await _service.Update(request, ExistingAmenity.Id);

            //Assert
            await _repository.Amenities.Received(1).GetById(ExistingAmenity.Id);
            _repository.Amenities.Received(1).Update(
                Arg.Is<Amenity>(
                    e => e.Id == ExistingAmenity.Id &&
                    e.Name == request.Name &&
                    e.NormalizedName == request.Name.ToUpper())
                );
            await _repository.Received(1).SaveAsync();
            Assert.Equal(request.Name, ExistingAmenity.Name);
        }

        [Fact]
        public async Task Update_ShouldThrowNotFoundException_WhenAmenityDoesntExist()
        {
            //Arrange
            var id = Guid.NewGuid();
            var request = new UpdateAmenityRequest { Name = "NewCooler" };
            _repository.Amenities.GetById(id).Returns(Task.FromResult<Amenity>(null));

            //Act
            var exception = await Assert.ThrowsAsync<NotFoundException>(() => _service.Update(request, id));

            //Assert
            Assert.NotNull(exception);
            Assert.Contains(id.ToString(), exception.ErrorMessage);
        }

        [Fact]
        public async Task Delete_ShouldDeleteAmenity_WhenRoleExist()
        {
            //Arrange
            _repository.Amenities.GetById(ExistingAmenity.Id).Returns(ExistingAmenity);

            // Act
            await _service.Delete(ExistingAmenity.Id);

            // Assert
            await _repository.Amenities.Received(1).GetById(ExistingAmenity.Id);
            _repository.Amenities.Received(1).Delete(
                Arg.Is<Amenity>(
                    e => e.Id == ExistingAmenity.Id)
                );
            await _repository.Received(1).SaveAsync();
        }

        [Fact]
        public async Task Delete_ShouldThrowNotFoundException_WhenRoleDoesntExist()
        {
            //Arrange
            var id = Guid.NewGuid();
            _repository.Amenities.GetById(id).Returns(Task.FromResult<Amenity>(null));

            //Act
            var exception = await Assert.ThrowsAsync<NotFoundException>(async () => await _service.Delete(id));

            //Assert
            Assert.NotNull(exception);
            Assert.Contains(id.ToString(), exception.ErrorMessage);
        }

        [Fact]
        public async Task GetById_ShouldReturnAmenityResponse_WhenAmenityExists()
        {
            //Arrange
            _repository.Amenities.GetById(ExistingAmenity.Id).Returns(ExistingAmenity);

            //Act
            var result = await _service.GetById(ExistingAmenity.Id);

            //Assert
            Assert.NotNull(result);
            Assert.IsType<AmenityResponse>(result);
            Assert.Equal(ExistingAmenity.Id, result.Id);
            Assert.Equal(ExistingAmenity.Name, result.Name);
        }

        [Fact]
        public async Task GetById_ShouldThrowNotFoundException_WhenAmenityDoesntExist()
        {
            //Arrange
            var id = Guid.NewGuid();
            _repository.Amenities.GetById(id).Returns(Task.FromResult<Amenity>(null));

            //Act
            var exception = await Assert.ThrowsAsync<NotFoundException>(async () => await _service.GetById(id));

            //Assert
            Assert.NotNull(exception);
            Assert.Contains(id.ToString(), exception.Message);
        }

        [Fact]
        public async Task GetAll_ShouldReturnsAllAmenitiesAsAmenityResponse_EvenIfAmenitiesDontExist()
        {
            //Arrange
            _repository.Amenities.GetAll().Returns(AllAmenities);

            //Act
            var amenitiesResponse = await _service.GetAll();

            //Assert
            Assert.NotNull(amenitiesResponse);
            Assert.IsType<List<AmenityResponse>>(amenitiesResponse);
            Assert.All(amenitiesResponse, amenity => Assert.NotNull(amenity.Name));
            Assert.Equal(AllAmenities.Count(), amenitiesResponse.Count());
        }

        private Amenity ExistingAmenity { get; }
        private IEnumerable<Amenity> AllAmenities;
    }
}
