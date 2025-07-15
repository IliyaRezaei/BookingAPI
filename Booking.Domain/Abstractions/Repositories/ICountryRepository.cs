using Booking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Domain.Abstractions.Repositories
{
    public interface ICountryRepository
    {
        public Task<IEnumerable<Country>> GetAll();
        public Task<Country?> GetById(Guid id);
        public Task<Country?> GetByName(string name);
        public Task Create(Country country);
        public void Delete(Country country);
        public void Update(Country country);
    }
}
