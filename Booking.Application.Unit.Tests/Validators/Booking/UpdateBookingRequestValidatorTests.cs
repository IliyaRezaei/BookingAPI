using Booking.Application.Utilities;
using Booking.Application.Validators.Booking;
using Booking.Domain.Abstractions.Repositories.Manager;
using Booking.Domain.Contracts.Booking;
using Booking.Domain.Entities;
using Booking.Domain.Utilities;
using FluentValidation;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.Tests.Unit.Validators.Booking
{
    public class UpdateBookingRequestValidatorTests
    {
        private readonly IRepositoryManager _repository;
        private readonly UpdateBookingRequestValidator _validator;
        private readonly ApplicationUser _client;
        private readonly Property _property;
        private readonly Domain.Entities.Booking _booking;
        public UpdateBookingRequestValidatorTests()
        {
            _repository = Substitute.For<IRepositoryManager>();
            _client = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = "email@gmail.com",
                NormalizedEmail = "EMAIL@GMAIL.COM",
                Username = "username",
                NormalizedUsername = "USERNAME",
                HashedPassword = "Password".HashPassword(),
                ImageUrl = "",
                IsHost = true,
            };
            _property = new Property
            {
                Id = Guid.NewGuid(),
                Title = "Title",
                Description = "Description",
                BedRooms = 1,
                Beds = 1,
                Bathrooms = 1,
                Guests = 1,
                Host = _client
            };
            _booking = new Domain.Entities.Booking
            {
                Id = Guid.NewGuid(),
                CheckIn = DateOnly.FromDateTime(DateTime.UtcNow),
                CheckOut = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)),
                Property = _property,
                Client = _client,
                TotalCost = BookingCalculator.CalculateTotalCost(
                    DateOnly.FromDateTime(DateTime.UtcNow),
                    DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)),
                    _property.PricePerNight),
                Status = Domain.Enums.BookingStatus.Pending
            };
            ExistingBooking = new Domain.Entities.Booking
            {
                Id = Guid.NewGuid(),
                CheckIn = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(4)),
                CheckOut = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(6)),
                Property = _property,
                Client = _client,
                TotalCost = BookingCalculator.CalculateTotalCost(
                    DateOnly.FromDateTime(DateTime.UtcNow.AddDays(4)),
                    DateOnly.FromDateTime(DateTime.UtcNow.AddDays(6)),
                    _property.PricePerNight),
                Status = Domain.Enums.BookingStatus.Pending
            };
            _validator = new UpdateBookingRequestValidator(_repository, _booking.Id,_property.Id);
        }

        [Theory]
        [MemberData(nameof(ValidBookingDates))]
        public async Task ShouldPass_WhenCheckInAndCheckOutDatesAreValid(DateOnly checkInDate, DateOnly checkOutDate)
        {
            //Arrange
            var request = new UpdateBookingRequest { CheckIn = checkInDate, CheckOut = checkOutDate };
            //Act
            var result = await _validator.ValidateAsync(request);
            //Assert
            Assert.True(result.IsValid);
        }

        [Theory]
        [MemberData(nameof(ValidBookingDatesWithInterference))]
        public async Task ShouldPass_WhenCheckInAndCheckOutDatesDontInterefereWithOtherBookings(DateOnly checkInDate, DateOnly checkOutDate)
        {
            //Arrange
            var request = new UpdateBookingRequest { CheckIn = checkInDate, CheckOut = checkOutDate };
            _repository.Bookings.GetAllUpcomingOfAPropertyById(Arg.Any<Guid>()).Returns(new List<Domain.Entities.Booking> { ExistingBooking });
            //Act
            var result = await _validator.ValidateAsync(request);
            //Assert
            Assert.True(result.IsValid);
        }

        [Theory]
        [MemberData(nameof(InvalidBookingDates))]
        public async Task ShouldFail_WhenCheckInAndCheckOutDatesAreInvalid(DateOnly checkInDate, DateOnly checkOutDate)
        {
            //Arrange
            var request = new UpdateBookingRequest { CheckIn = checkInDate, CheckOut = checkOutDate };
            //Act
            var result = await _validator.ValidateAsync(request);
            //Assert
            Assert.False(result.IsValid);
        }

        [Theory]
        [MemberData(nameof(InvalidBookingDatesWithInterference))]
        public async Task ShouldFail_WhenCheckInAndCheckOutDatesInterfereWithAnotherBooking(DateOnly checkInDate, DateOnly checkOutDate)
        {
            //Arrange
            var request = new UpdateBookingRequest { CheckIn = checkInDate, CheckOut = checkOutDate };
            _repository.Bookings.GetAllUpcomingOfAPropertyById(Arg.Any<Guid>()).Returns(new List<Domain.Entities.Booking> { ExistingBooking });
            //Act
            var result = await _validator.ValidateAsync(request);
            //Assert
            Assert.False(result.IsValid);
        }

        private Domain.Entities.Booking ExistingBooking { get; set; }

        public static IEnumerable<object[]> ValidBookingDates =>
            new List<object[]>
            {
                new object[] { DateOnly.FromDateTime(DateTime.UtcNow), DateOnly.FromDateTime(DateTime.UtcNow).AddDays(2) },
                new object[] { DateOnly.FromDateTime(DateTime.UtcNow).AddDays(4), DateOnly.FromDateTime(DateTime.UtcNow).AddDays(6) },
            };

        public static IEnumerable<object[]> ValidBookingDatesWithInterference =>
            new List<object[]>
            {
                new object[] { DateOnly.FromDateTime(DateTime.UtcNow), DateOnly.FromDateTime(DateTime.UtcNow).AddDays(3) },
                new object[] { DateOnly.FromDateTime(DateTime.UtcNow).AddDays(7), DateOnly.FromDateTime(DateTime.UtcNow).AddDays(9) }
            };

        public static IEnumerable<object[]> InvalidBookingDates =>
            new List<object[]>
            {
                new object[] { DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-2), DateOnly.FromDateTime(DateTime.UtcNow).AddDays(2) },
                new object[] { DateOnly.FromDateTime(DateTime.UtcNow).AddDays(31), DateOnly.FromDateTime(DateTime.UtcNow).AddDays(35) }
            };

        public static IEnumerable<object[]> InvalidBookingDatesWithInterference =>
            new List<object[]>
            {
                new object[] { DateOnly.FromDateTime(DateTime.UtcNow), DateOnly.FromDateTime(DateTime.UtcNow).AddDays(5) },
                new object[] { DateOnly.FromDateTime(DateTime.UtcNow).AddDays(3), DateOnly.FromDateTime(DateTime.UtcNow).AddDays(6) }
            };
    }
}
