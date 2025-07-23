using Booking.Domain.Contracts.Booking;
using Booking.Domain.Entities;
using Booking.Domain.Utilities;

namespace Booking.Application.Mappers
{
    internal static class BookingMappers
    {
        public static Domain.Entities.Booking ToEntity(this CreateBookingRequest request, ApplicationUser client, Property property)
        {
            return new Domain.Entities.Booking
            {
                Id = Guid.NewGuid(),
                CheckIn = request.CheckIn,
                CheckOut = request.CheckOut,
                Client = client,
                Property = property,
                TotalCost = BookingCalculator.CalculateTotalCost(request.CheckIn, request.CheckOut, property.PricePerNight),
                Status = Domain.Enums.BookingStatus.Pending
            };
        }
        public static Domain.Entities.Booking ToEntity(this UpdateBookingRequest request, Guid id, ApplicationUser client, Property property)
        {
            return new Domain.Entities.Booking
            {
                Id = id,
                CheckIn = request.CheckIn,
                CheckOut = request.CheckOut,
                Client = client,
                Property = property,
                TotalCost = BookingCalculator.CalculateTotalCost(request.CheckIn, request.CheckOut, property.PricePerNight),
                Status = Domain.Enums.BookingStatus.Pending,
            };
        }

        public static BookingResponse ToResponse(this Domain.Entities.Booking entity)
        {
            return new BookingResponse
            {
                Id = entity.Id,
                CheckIn = entity.CheckIn,
                CheckOut = entity.CheckOut,
                Status = entity.Status,
                TotalCost = entity.TotalCost
            };
        }
        public static IEnumerable<BookingResponse> ToResponse(this IEnumerable<Domain.Entities.Booking> bookings)
        {
            return bookings.Select(b => b.ToResponse()).ToList();
        }
    }
}
