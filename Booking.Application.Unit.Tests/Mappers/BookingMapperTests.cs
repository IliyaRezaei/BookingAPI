using Booking.Application.Mappers;
using Booking.Application.Utilities;
using Booking.Domain.Contracts.Booking;
using Booking.Domain.Entities;
using Booking.Domain.Enums;
using Booking.Domain.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.Tests.Unit.Mappers
{
    public class BookingMapperTests
    {

        public BookingMapperTests()
        {
            ExistingClient = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Username = "Username",
                NormalizedUsername = "USERNAME",
                Email = "Email@gmail.com",
                NormalizedEmail = "EMAIL@GMAIL.COM",
                HashedPassword = "Test1234@".HashPassword(),
                ImageUrl = "",
                IsHost = false
            };
            ExistingProperty = new Property
            {
                Id = Guid.NewGuid(),
                Title = "Title",
                Description = "Description",
                PricePerNight = 1000,
                Bathrooms = 1,
                Beds = 1,
                Guests = 1,
                BedRooms = 1,
            };
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void ToEntity_ShouldReturnValidBookingEntity_FromCreateBookingRequest(int days)
        {
            //Arrange
            var request = new CreateBookingRequest
            {
                CheckIn = DateOnly.FromDateTime(DateTime.Now),
                CheckOut = DateOnly.FromDateTime(DateTime.Now.AddDays(days))
            };

            //Act
            var entity = request.ToEntity(ExistingClient, ExistingProperty);

            //Assert
            Assert.NotNull(entity);
            Assert.Equal(entity.CheckIn, request.CheckIn);
            Assert.Equal(entity.CheckOut, request.CheckOut);
            Assert.Equal(BookingStatus.Pending, entity.Status);
            Assert.Equal(entity.Property, ExistingProperty);
            Assert.Equal(entity.Client, ExistingClient);
            Assert.Equal((days * entity.Property.PricePerNight), entity.TotalCost);
        }

        [Fact]
        public void ToEntity_ShouldReturnValidBookingEntity_FromUpdateBookingRequest()
        {
            //Arrange
            var bookingId = Guid.NewGuid();
            var request = new UpdateBookingRequest
            {
                CheckIn = DateOnly.FromDateTime(DateTime.Now),
                CheckOut = DateOnly.FromDateTime(DateTime.Now.AddDays(3))
            };

            //Act
            var entity = request.ToEntity(bookingId, ExistingClient, ExistingProperty);

            //Assert
            Assert.NotNull(entity);
            Assert.Equal(entity.CheckIn, request.CheckIn);
            Assert.Equal(entity.CheckOut, request.CheckOut);
            Assert.Equal(BookingStatus.Pending, entity.Status);
            Assert.Equal(BookingCalculator.CalculateTotalCost(
                request.CheckIn, request.CheckOut, entity.Property.PricePerNight),
                entity.TotalCost);
        }

        [Fact]
        public void ToResponse_ShouldReturnValidBookingResponse_FromBookingEntity()
        {
            //Arrange
            var entity = new Domain.Entities.Booking
            {
                Id = Guid.NewGuid(),
                CheckIn = DateOnly.FromDateTime(DateTime.UtcNow),
                CheckOut = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)),
                Client = ExistingClient,
                Property = ExistingProperty,
                Status = BookingStatus.Pending,
                TotalCost = BookingCalculator.CalculateTotalCost(
                    DateOnly.FromDateTime(DateTime.UtcNow),
                    DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)),
                    ExistingProperty.PricePerNight)
            };

            //Act
            var response = entity.ToResponse();

            //Assert
            Assert.NotNull(response);
            Assert.IsType<BookingResponse>(response);
            Assert.Equal(response.CheckIn, entity.CheckIn);
            Assert.Equal(response.CheckOut, entity.CheckOut);
            Assert.Equal(response.Id, entity.Id);
        }

        [Fact]
        public void ToResponse_ShouldReturnValidBookingResponses_FromBookingEntities()
        {
            //Arrange
            var entities = new List<Domain.Entities.Booking>
            {
                new Domain.Entities.Booking
                {
                    Id = Guid.NewGuid(),
                    CheckIn = DateOnly.FromDateTime(DateTime.UtcNow),
                    CheckOut = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)),
                    Client = ExistingClient,
                    Property = ExistingProperty,
                    Status = BookingStatus.Pending,
                    TotalCost = BookingCalculator.CalculateTotalCost(
                        DateOnly.FromDateTime(DateTime.UtcNow),
                        DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)),
                        ExistingProperty.PricePerNight)
                },  new Domain.Entities.Booking
                {
                    Id = Guid.NewGuid(),
                    CheckIn = DateOnly.FromDateTime(DateTime.UtcNow),
                    CheckOut = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)),
                    Client = ExistingClient,
                    Property = ExistingProperty,
                    Status = BookingStatus.Pending,
                    TotalCost = BookingCalculator.CalculateTotalCost(
                        DateOnly.FromDateTime(DateTime.UtcNow),
                        DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)),
                        ExistingProperty.PricePerNight)
                },  new Domain.Entities.Booking
                {
                    Id = Guid.NewGuid(),
                    CheckIn = DateOnly.FromDateTime(DateTime.UtcNow),
                    CheckOut = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)),
                    Client = ExistingClient,
                    Property = ExistingProperty,
                    Status = BookingStatus.Pending,
                    TotalCost = BookingCalculator.CalculateTotalCost(
                        DateOnly.FromDateTime(DateTime.UtcNow),
                        DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)),
                        ExistingProperty.PricePerNight)
                }
            };

            //Act
            var responses = entities.ToResponse();

            //Assert
            Assert.NotNull(responses);
            Assert.IsType<List<BookingResponse>>(responses);
            Assert.All(entities, entity => Assert.Contains(responses, booking => booking.Id == entity.Id));
            Assert.All(entities, entity => Assert.Contains(responses, booking => booking.CheckIn == entity.CheckIn));
            Assert.All(entities, entity => Assert.Contains(responses, booking => booking.CheckOut == entity.CheckOut));
        }

        private ApplicationUser ExistingClient { get; }
        private Property ExistingProperty { get; }
    }
}
