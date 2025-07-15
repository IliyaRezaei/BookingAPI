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
    internal class CityRepository : ICityRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public CityRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Create(City city)
        {
            await _dbContext.Cities.AddAsync(city);
        }

        public void Delete(City city)
        {
            _dbContext.Cities.Remove(city);
        }

        public async Task<IEnumerable<City>> GetAll()
        {
            return await _dbContext.Cities.AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<City>> GetAllByCountryId(Guid countryId)
        {
            return await _dbContext.Cities.Where(c => c.Country.Id == countryId).AsNoTracking().ToListAsync();
        }

        public async Task<City?> GetById(Guid id)
        {
            return await _dbContext.Cities.FindAsync(id);
        }

        public async Task<City?> GetByName(string name)
        {
            return await _dbContext.Cities.Include(c => c.Country).FirstOrDefaultAsync(c => c.Name == name);
        }

        public void Update(City city)
        {
            _dbContext.Cities.Update(city);
        }
    }
}
