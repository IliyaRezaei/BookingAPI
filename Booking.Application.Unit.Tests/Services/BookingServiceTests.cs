using Booking.Application.Errors;
using Booking.Application.Services;
using Booking.Application.Utilities;
using Booking.Domain.Abstractions.Repositories.Manager;
using Booking.Domain.Contracts.Booking;
using Booking.Domain.Entities;
using Booking.Domain.Enums;
using Booking.Domain.Errors;
using Booking.Domain.Utilities;
using FluentValidation;
using NSubstitute;
using System;

namespace Booking.Application.Tests.Unit.Services
{
    public class BookingServiceTests
    {
        private readonly IRepositoryManager _repository;
        private readonly BookingService _service;

        public BookingServiceTests()
        {
            _repository = Substitute.For<IRepositoryManager>();
            _service = new BookingService(_repository);
            ExistingClient = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Username = "Username",
                NormalizedUsername = "USERNAME",
                Email = "Email@gmail.com",
                NormalizedEmail = "EMAIL@GMAIL.COM",
                HashedPassword = "Password1234@".HashPassword(),
                ImageUrl = "",
                IsHost = true,
            };
            ExistingProperty = new Property
            {
                Id = Guid.NewGuid(),
                Title = "Title",
                Description = "Description",
                BedRooms = 1,
                Beds = 1,
                Bathrooms = 1,
                Guests = 1,
                Host = ExistingClient
            };
            ExistingBooking = new Domain.Entities.Booking
            {
                Id = Guid.NewGuid(),
                Client = ExistingClient,
                Property = ExistingProperty,
                CheckIn = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
                CheckOut = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
                Status = BookingStatus.Pending,
                TotalCost = BookingCalculator.CalculateTotalCost(
                    DateOnly.FromDateTime(DateTime.UtcNow),
                    DateOnly.FromDateTime(DateTime.UtcNow).AddDays(5),
                    ExistingProperty.PricePerNight)
            };
            AllBookings = new List<Domain.Entities.Booking>
            {
                new Domain.Entities.Booking
                {
                    Id = Guid.NewGuid(),
                    Client = ExistingClient,
                    Property = ExistingProperty,
                    CheckIn = DateOnly.FromDateTime(DateTime.UtcNow),
                    CheckOut = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)),
                    Status = BookingStatus.Pending,
                    TotalCost = BookingCalculator.CalculateTotalCost(
                        DateOnly.FromDateTime(DateTime.UtcNow),
                        DateOnly.FromDateTime(DateTime.UtcNow).AddDays(3),
                        ExistingProperty.PricePerNight)
                },new Domain.Entities.Booking
                {
                    Id = Guid.NewGuid(),
                    Client = ExistingClient,
                    Property = ExistingProperty,
                    CheckIn = DateOnly.FromDateTime(DateTime.UtcNow),
                    CheckOut = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
                    Status = BookingStatus.Pending,
                    TotalCost = BookingCalculator.CalculateTotalCost(
                        DateOnly.FromDateTime(DateTime.UtcNow),
                        DateOnly.FromDateTime(DateTime.UtcNow).AddDays(5),
                        ExistingProperty.PricePerNight)
                }
            };
        }

        [Fact]
        public async Task Create_ShouldCreateAndReturnBookingResponse_WhenRequestIsValid()
        {
            //Arrange
            var request = new CreateBookingRequest
            {
                CheckIn = DateOnly.FromDateTime(DateTime.UtcNow),
                CheckOut = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3))
            };
            _repository.Users.GetByUsername(ExistingClient.Username).Returns(ExistingClient);
            _repository.Properties.GetById(ExistingProperty.Id).Returns(ExistingProperty);
            _repository.Bookings.GetAllUpcomingOfAPropertyById(ExistingProperty.Id)
                .Returns(Task.FromResult<IEnumerable<Domain.Entities.Booking>>(
                    new List<Domain.Entities.Booking> { }));

            //Assert
            var response = await _service.Create(request, ExistingClient.Username, ExistingProperty.Id);

            //Act
            Assert.NotNull(response);
            Assert.IsType<BookingResponse>(response);
            Assert.Equal(request.CheckIn, response.CheckIn);
            Assert.Equal(request.CheckOut, response.CheckOut);
            Assert.Equal(
                BookingCalculator.CalculateTotalCost(request.CheckIn, request.CheckOut, ExistingProperty.PricePerNight), 
                response.TotalCost);
            Assert.Equal(BookingStatus.Pending, response.Status);
        }

        [Fact]
        public async Task Create_ShouldThrowValidationException_WhenRequestIsNotValid()
        {
            //Arrange
            var request = new CreateBookingRequest
            {
                CheckIn = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)),
                CheckOut = DateOnly.FromDateTime(DateTime.UtcNow)
            };
            _repository.Users.GetByUsername(ExistingClient.Username).Returns(ExistingClient);
            _repository.Properties.GetById(ExistingProperty.Id).Returns(ExistingProperty);
            _repository.Bookings.GetAllUpcomingOfAPropertyById(ExistingProperty.Id)
                .Returns(Task.FromResult<IEnumerable<Domain.Entities.Booking>>(
                    new List<Domain.Entities.Booking> { }));

            //Assert
            var exception = await Assert.ThrowsAsync<ValidationException>(async () =>
            await _service.Create(request, ExistingClient.Username, ExistingProperty.Id));

            //Act
            Assert.NotNull(exception);
            Assert.IsType<ValidationException>(exception);
        }

        [Fact]
        public async Task Create_ShouldThrowUnauthorizedException_WhenNoUserWithThatUsernameExist()
        {
            //Arrange
            var request = new CreateBookingRequest
            {
                CheckIn = DateOnly.FromDateTime(DateTime.UtcNow),
                CheckOut = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3))
            };
            _repository.Users.GetByUsername(ExistingClient.Username).Returns(Task.FromResult<ApplicationUser>(null));
            _repository.Properties.GetById(ExistingProperty.Id).Returns(ExistingProperty);
            _repository.Bookings.GetAllUpcomingOfAPropertyById(ExistingProperty.Id)
                .Returns(Task.FromResult<IEnumerable<Domain.Entities.Booking>>(
                    new List<Domain.Entities.Booking> { }));

            //Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedException>(async() =>
            await _service.Create(request, ExistingClient.Username, ExistingProperty.Id));

            //Act
            Assert.NotNull(exception);
            Assert.IsType<UnauthorizedException>(exception);
        }

        [Fact]
        public async Task Create_ShouldThrowNotFoundException_WhenPropertyWithThatPropertyIdDontExist()
        {
            //Arrange
            var request = new CreateBookingRequest
            {
                CheckIn = DateOnly.FromDateTime(DateTime.UtcNow),
                CheckOut = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3))
            };
            _repository.Users.GetByUsername(ExistingClient.Username).Returns(ExistingClient);
            _repository.Properties.GetById(ExistingProperty.Id).Returns(Task.FromResult<Property>(null));
            _repository.Bookings.GetAllUpcomingOfAPropertyById(ExistingProperty.Id)
                .Returns(Task.FromResult<IEnumerable<Domain.Entities.Booking>>(
                    new List<Domain.Entities.Booking> { }));

            //Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(async () =>
            await _service.Create(request, ExistingClient.Username, ExistingProperty.Id));

            //Act
            Assert.NotNull(exception);
            Assert.IsType<NotFoundException>(exception);
        }

        [Fact]
        public async Task Create_ShouldThrowForbiddenException_WhenBookingYourOwnProperty()
        {
            //Arrange
            var request = new CreateBookingRequest
            {
                CheckIn = DateOnly.FromDateTime(DateTime.UtcNow),
                CheckOut = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3))
            };
            _repository.Users.GetByUsername(ExistingClient.Username).Returns(ExistingClient);
            _repository.Properties.GetById(ExistingProperty.Id).Returns(ExistingProperty);
            _repository.Properties.GetAllPropertiesOfAUser(ExistingClient).Returns(new List<Property> { ExistingProperty });
            _repository.Bookings.GetAllUpcomingOfAPropertyById(ExistingProperty.Id)
                .Returns(Task.FromResult<IEnumerable<Domain.Entities.Booking>>(
                    new List<Domain.Entities.Booking> { }));

            //Assert
            var exception = await Assert.ThrowsAsync<ForbiddenException>(async () =>
            await _service.Create(request, ExistingClient.Username, ExistingProperty.Id));

            //Act
            Assert.NotNull(exception);
            Assert.IsType<ForbiddenException>(exception);
        }

        [Fact]
        public async Task Update_ShouldUpdate_WhenRequestIsValid()
        {
            //Arrange
            var request = new UpdateBookingRequest
            {
                CheckIn = DateOnly.FromDateTime(DateTime.UtcNow),
                CheckOut = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3))
            };
            _repository.Users.GetByUsername(ExistingClient.Username).Returns(ExistingClient);
            _repository.Properties.GetById(ExistingProperty.Id).Returns(ExistingProperty);
            _repository.Bookings.GetById(ExistingBooking.Id).Returns(ExistingBooking);
            _repository.Bookings.GetAllUpcomingOfAPropertyById(ExistingProperty.Id)
                .Returns( new List<Domain.Entities.Booking> { ExistingBooking });
            _repository.Bookings.GetAllBookingsOfAUser(ExistingClient)
                .Returns(new List<Domain.Entities.Booking> { ExistingBooking });

            //Act
            await _service.Update(request, ExistingBooking.Id, ExistingClient.Username, ExistingProperty.Id);

            //Assert
            await _repository.Users.Received(1).GetByUsername(ExistingClient.Username);
            await _repository.Properties.Received(1).GetById(ExistingProperty.Id);
            await _repository.Bookings.Received(1).GetAllUpcomingOfAPropertyById(ExistingProperty.Id);
            await _repository.Bookings.Received(1).GetAllBookingsOfAUser(ExistingClient);
            await _repository.Received(1).SaveAsync();
        }

        [Fact]
        public async Task Update_ShouldThrowValidationException_WhenInvalidRequest()
        {
            //Arrange
            var request = new UpdateBookingRequest
            {
                CheckIn = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)),
                CheckOut = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1))
            };
            _repository.Users.GetByUsername(ExistingClient.Username).Returns(ExistingClient);
            _repository.Properties.GetById(ExistingProperty.Id).Returns(ExistingProperty);
            _repository.Bookings.GetById(ExistingBooking.Id).Returns(ExistingBooking);
            _repository.Bookings.GetAllUpcomingOfAPropertyById(ExistingProperty.Id)
                .Returns(new List<Domain.Entities.Booking> { ExistingBooking });
            _repository.Bookings.GetAllBookingsOfAUser(ExistingClient)
                .Returns(new List<Domain.Entities.Booking> { ExistingBooking });

            //Act
            var exception = await Assert.ThrowsAsync<ValidationException>(async () =>
            await _service.Update(request, ExistingBooking.Id, ExistingClient.Username, ExistingProperty.Id));

            //Assert
            Assert.NotNull(exception);
            Assert.IsType<ValidationException>(exception);
        }

        [Fact]
        public async Task Update_ShouldThrowValidationException_WhenBookingDatesInterfere()
        {
            //Arrange
            var request = new UpdateBookingRequest
            {
                CheckIn = DateOnly.FromDateTime(DateTime.UtcNow),
                CheckOut = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3))
            };
            var booking = new Domain.Entities.Booking
            {
                Id = Guid.NewGuid(),
                CheckIn = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)),
                CheckOut = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(4)),
                Client = ExistingClient,
                Property = ExistingProperty,
                Status = BookingStatus.Pending,
                TotalCost = BookingCalculator.CalculateTotalCost(
                    DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)),
                    DateOnly.FromDateTime(DateTime.UtcNow.AddDays(4)),
                    ExistingProperty.PricePerNight)
            };
            _repository.Users.GetByUsername(ExistingClient.Username).Returns(ExistingClient);
            _repository.Properties.GetById(ExistingProperty.Id).Returns(ExistingProperty);
            _repository.Bookings.GetById(ExistingBooking.Id).Returns(ExistingBooking);
            _repository.Bookings.GetAllUpcomingOfAPropertyById(ExistingProperty.Id)
                .Returns(new List<Domain.Entities.Booking> { ExistingBooking, booking });
            _repository.Bookings.GetAllBookingsOfAUser(ExistingClient)
                .Returns(new List<Domain.Entities.Booking> { ExistingBooking });

            //Act
            var exception = await Assert.ThrowsAsync<ValidationException>(async () =>
            await _service.Update(request, ExistingBooking.Id, ExistingClient.Username, ExistingProperty.Id));

            //Assert
            Assert.NotNull(exception);
            Assert.IsType<ValidationException>(exception);
        }

        [Fact]
        public async Task Update_ShouldThrowNotFoundException_WhenPropertyIsNonexistence()
        {
            //Arrange
            var request = new UpdateBookingRequest
            {
                CheckIn = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
                CheckOut = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3))
            };
            _repository.Users.GetByUsername(ExistingClient.Username).Returns(ExistingClient);
            _repository.Properties.GetById(ExistingProperty.Id).Returns(Task.FromResult<Property>(null));
            _repository.Bookings.GetById(ExistingBooking.Id).Returns(ExistingBooking);
            _repository.Bookings.GetAllUpcomingOfAPropertyById(ExistingProperty.Id)
                .Returns(new List<Domain.Entities.Booking> { ExistingBooking });
            _repository.Bookings.GetAllBookingsOfAUser(ExistingClient)
                .Returns(new List<Domain.Entities.Booking> { ExistingBooking });

            //Act
            var exception = await Assert.ThrowsAsync<NotFoundException>(async () =>
            await _service.Update(request, ExistingBooking.Id, ExistingClient.Username, ExistingProperty.Id));

            //Assert
            Assert.NotNull(exception);
            Assert.IsType<NotFoundException>(exception);
            Assert.Contains(ExistingProperty.Id.ToString(), exception.ErrorMessage);
        }

        [Fact]
        public async Task Delete_ShouldDeleteBooking_WhenRequestIsValidAndBookingIsClients()
        {
            //Arrange
            var bookingId = ExistingBooking.Id;
            var username = ExistingClient.Username;
            _repository.Bookings.GetAllBookingsOfAUser(ExistingClient)
                .Returns(new List<Domain.Entities.Booking> { ExistingBooking });
            _repository.Users.GetByUsername(username).Returns(ExistingClient);

            //Act
            await _service.Delete(bookingId, username);

            //Assert
            _repository.Bookings.Received(1).GetAllBookingsOfAUser(ExistingClient);
            _repository.Users.Received(1).GetByUsername(username);
            _repository.Bookings.Received(1).Delete(ExistingBooking);
            _repository.Received(1).SaveAsync();
        }

        [Fact]
        public async Task Delete_ShouldThrowNotFoundException_WhenRequestIsValidButTheBookingIsNotClients()
        {
            //Arrange
            var bookingId = ExistingBooking.Id;
            var username = ExistingClient.Username;
            _repository.Bookings.GetAllBookingsOfAUser(ExistingClient)
                .Returns(new List<Domain.Entities.Booking> { }); ;
            _repository.Users.GetByUsername(username).Returns(ExistingClient);

            //Act
            var exception = await Assert.ThrowsAsync<NotFoundException>(async () =>
            await _service.Delete(bookingId, username));

            //Assert
            _repository.Bookings.Received(1).GetAllBookingsOfAUser(ExistingClient);
            _repository.Users.Received(1).GetByUsername(username);
            Assert.NotNull(exception);
            Assert.IsType<NotFoundException>(exception);
            Assert.Contains(bookingId.ToString(), exception.ErrorMessage);
        }

        [Fact]
        public async Task Delete_ShouldThrowUnauthorizedException_WhenUserWithThatUsernameDoesntExist()
        {
            //Arrange
            var bookingId = ExistingBooking.Id;
            var username = ExistingClient.Username;
            _repository.Bookings.GetAllBookingsOfAUser(ExistingClient)
                .Returns(new List<Domain.Entities.Booking> { });
            _repository.Users.GetByUsername(username).Returns(Task.FromResult<ApplicationUser>(null));

            //Act
            var exception = await Assert.ThrowsAsync<UnauthorizedException>(async () =>
            await _service.Delete(bookingId, ""));

            //Assert
            Assert.NotNull(exception);
            Assert.IsType<UnauthorizedException>(exception);
        }

        [Fact]
        public async Task GetById_ShouldReturnBookingAsBookingResponse_WhenItExists()
        {
            //Arrange
            var bookingId = ExistingBooking.Id;
            _repository.Bookings.GetById(bookingId).Returns(ExistingBooking);

            //Act
            var response = await _service.GetById(bookingId);

            //Assert
            Assert.NotNull(response);
            Assert.IsType<BookingResponse>(response);
        }

        [Fact]
        public async Task GetById_ShouldThrowNotFoundException_WhenItDoesntExist()
        {
            //Arrange
            var bookingId = ExistingBooking.Id;
            _repository.Bookings.GetById(bookingId).Returns(Task.FromResult<Domain.Entities.Booking>(null));

            //Act
            var exception = await Assert.ThrowsAsync<NotFoundException>(async () =>
            await _service.GetById(bookingId));

            //Assert
            Assert.NotNull(exception);
            Assert.IsType<NotFoundException>(exception);
        }

        [Fact]
        public async Task GetAll_ShouldReturnBookingsAsBookingResponses_EvenIfBookingsDontExist()
        {
            //Arrange
            _repository.Bookings.GetAll().Returns(AllBookings);

            //Act
            var responses = await _service.GetAll();

            //Assert
            Assert.NotNull(responses);
            Assert.IsType<List<BookingResponse>>(responses);
            Assert.Equal(AllBookings.Count(), responses.Count());
        }

        private ApplicationUser ExistingClient { get; set; }
        private Property ExistingProperty { get; set; }
        private Domain.Entities.Booking ExistingBooking { get; set; }
        private List<Domain.Entities.Booking> AllBookings { get; set; }
    }
}
