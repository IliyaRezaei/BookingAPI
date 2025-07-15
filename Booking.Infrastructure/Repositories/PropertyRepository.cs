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
    internal class PropertyRepository : IPropertyRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public PropertyRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Create(Property property)
        {
            await _dbContext.Properties.AddAsync(property);
        }

        public void Delete(Property property)
        {
            _dbContext.Properties.Remove(property);
        }

        public async Task<IEnumerable<Property>> GetAll()
        {
            return await _dbContext.Properties
                .Include(p => p.Reviews)
                .Include(p => p.Amenities)
                .Include(p => p.Images).ToListAsync();
        }

        public async Task<IEnumerable<Property>> GetAllPropertiesOfAUser(ApplicationUser user)
        {
            return await _dbContext.Properties.Where(p => p.Host == user).ToListAsync();
        }

        public async Task<Property?> GetById(Guid id)
        {
            return await _dbContext.Properties.FindAsync(id);
        }

        public void Update(Property property)
        {
            _dbContext.Properties.Update(property);
        }
    }
}
