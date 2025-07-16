using Booking.Domain.Abstractions.Repositories;
using Booking.Domain.Entities;
using Booking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Azure.Core.HttpHeader;

namespace Booking.Infrastructure.Repositories
{
    internal class AmenityRepository : IAmenityRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public AmenityRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Create(Amenity amenity)
        {
            await _dbContext.Amenities.AddAsync(amenity);
        }

        public void Delete(Amenity amenity)
        {
            _dbContext.Amenities.Remove(amenity);
        }

        public async Task<IEnumerable<Amenity>> GetAll()
        {
            return await _dbContext.Amenities.AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<Amenity>> GetAllByIds(IEnumerable<Guid> ids)
        {
            return await _dbContext.Amenities.Where(a => ids.Contains(a.Id)).ToListAsync();
        }

        public async Task<IEnumerable<Amenity>> GetAllByNames(IEnumerable<string> names)
        {
            return await _dbContext.Amenities.Where(a => names.Contains(a.Name)).ToListAsync();
        }

        public async Task<Amenity?> GetById(Guid id)
        {
            return await _dbContext.Amenities.FindAsync(id);
        }

        public async Task<Amenity?> GetByName(string name)
        {
            return await _dbContext.Amenities.Where(a => a.NormalizedName == name.ToUpper())
                .FirstOrDefaultAsync();
        }

        public void Update(Amenity amenity)
        {
            _dbContext.Amenities.Update(amenity);
        }
    }
}
