using Booking.Domain.Contracts.Booking;
using Booking.Domain.Contracts.Image;
using Booking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.Mappers
{
    internal static class BookingMappers
    {
        public static Domain.Entities.Booking ToEntity(this CreateBookingRequest request, ApplicationUser client, Property property, int totalCost)
        {
            return new Domain.Entities.Booking
            {
                Id = Guid.NewGuid(),
                CheckIn = request.CheckIn,
                CheckOut = request.CheckOut,
                Client = client,
                Property = property,
                TotalCost = totalCost,
                Status = Domain.Enums.BookingStatus.Pending,
            };
        }
        public static Domain.Entities.Booking ToEntity(this UpdateBookingRequest request, Guid id, ApplicationUser client, Property property, int totalCost)
        {
            return new Domain.Entities.Booking
            {
                Id = id,
                CheckIn = request.CheckIn,
                CheckOut = request.CheckOut,
                Status = Domain.Enums.BookingStatus.Pending,
                Client = client,
                Property = property,
                TotalCost = totalCost
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
            return bookings.Select(b => b.ToResponse());
        }
    }
}
