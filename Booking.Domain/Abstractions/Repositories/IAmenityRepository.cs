using Booking.Domain.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Domain.Abstractions.Repositories
{
    public interface IAmenityRepository
    {
        public Task<IEnumerable<Amenity>> GetAll();
        public Task<IEnumerable<Amenity>> GetAllByNames(IEnumerable<string> names);
        public Task<IEnumerable<Amenity>> GetAllByIds(IEnumerable<Guid> ids);
        public Task<Amenity?> GetById(Guid id);
        public Task<Amenity?> GetByName(string name);
        public Task Create(Amenity amenity);
        public void Delete(Amenity amenity);
        public void Update(Amenity amenity);
    }
}
