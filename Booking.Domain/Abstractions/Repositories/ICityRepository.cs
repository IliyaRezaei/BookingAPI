using Booking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Domain.Abstractions.Repositories
{
    public interface ICityRepository
    {
        public Task<IEnumerable<City>> GetAll();
        public Task<IEnumerable<City>> GetAllByCountryId(Guid countryId);
        public Task<City?> GetById(Guid id);
        public Task<City?> GetByName(string name);
        public Task Create(City city);
        public void Delete(City city);
        public void Update(City city);
    }
}
