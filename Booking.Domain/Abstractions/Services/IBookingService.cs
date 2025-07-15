using Booking.Domain.Contracts.Booking;
using Booking.Domain.Contracts.Review;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Domain.Abstractions.Services
{
    public interface IBookingService
    {
        public Task<IEnumerable<BookingResponse>> GetAll();
        public Task<BookingResponse> GetById(Guid id);
        public Task<BookingResponse> Create(CreateBookingRequest request, string username, Guid propertyId);
        public Task Delete(Guid id, string username);
        public Task Update(UpdateBookingRequest request, Guid id, string username, Guid propertyId);
    }
}
