using Booking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Domain.Abstractions.Repositories
{
    public interface IBookingRepository
    {
        public Task<IEnumerable<Entities.Booking>> GetAll();
        public Task<IEnumerable<Entities.Booking>> GetAllBookingsOfAUser(ApplicationUser user);
        public Task<IEnumerable<Entities.Booking>> GetAllUpcomingOfAPropertyById(Guid propertyId);
        public Task<Entities.Booking?> GetById(Guid id);
        public Task Create(Entities.Booking booking);
        public void Delete(Entities.Booking booking);
        public void Update(Entities.Booking booking);
    }
}
