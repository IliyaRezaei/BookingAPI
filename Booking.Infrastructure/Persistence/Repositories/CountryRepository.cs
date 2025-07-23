using Booking.Domain.Abstractions.Repositories;
using Booking.Domain.Entities;
using Booking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Infrastructure.Persistence.Repositories
{
    internal class CountryRepository : ICountryRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public CountryRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Create(Country country)
        {
            await _dbContext.Countries.AddAsync(country);
        }

        public void Delete(Country country)
        {
            _dbContext.Countries.Remove(country);
        }

        public async Task<IEnumerable<Country>> GetAll()
        {
            return await _dbContext.Countries.Include(c => c.Cities).AsNoTracking().ToListAsync();
        }

        public async Task<Country?> GetById(Guid id)
        {
            return await _dbContext.Countries.FindAsync(id);
        }

        public async Task<Country?> GetByName(string name)
        {
            return await _dbContext.Countries.Where(c => c.NormalizedName == name.ToUpper())
                .FirstOrDefaultAsync();
        }

        public void Update(Country country)
        {
            _dbContext.Countries.Update(country);
        }
    }
}
