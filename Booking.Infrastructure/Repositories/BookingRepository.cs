using Booking.Domain.Abstractions.Repositories;
using Booking.Domain.Entities;
using Booking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Infrastructure.Repositories
{
    internal class BookingRepository : IBookingRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public BookingRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Create(Domain.Entities.Booking booking)
        {
            await _dbContext.Bookings.AddAsync(booking);
        }

        public void Delete(Domain.Entities.Booking booking)
        {
            _dbContext.Bookings.Remove(booking);
        }

        public async Task<IEnumerable<Domain.Entities.Booking>> GetAll()
        {
            return await _dbContext.Bookings.ToListAsync();
        }

        public async Task<IEnumerable<Domain.Entities.Booking>> GetAllBookingsOfAUser(ApplicationUser user)
        {
            return await _dbContext.Bookings.Where(b => b.Client == user).ToListAsync();
        }

        public async Task<IEnumerable<Domain.Entities.Booking>> GetAllUpcomingOfAPropertyById(Guid propertyId)
        {
            return await _dbContext.Bookings.Include(b => b.Property).Where(
                b => b.Property.Id == propertyId &&
                b.CheckOut >= DateOnly.FromDateTime(DateTime.Today)
            ).ToListAsync();
        }

        public async Task<Domain.Entities.Booking?> GetById(Guid id)
        {
            return await _dbContext.Bookings.FindAsync(id);
        }

        public void Update(Domain.Entities.Booking booking)
        {
            _dbContext.Bookings.Update(booking);
        }
    }
}
