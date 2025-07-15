using Booking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Domain.Abstractions.Repositories
{
    public interface IPropertyRepository
    {
        public Task<IEnumerable<Property>> GetAll();
        public Task<IEnumerable<Property>> GetAllPropertiesOfAUser(ApplicationUser user);
        public Task<Property?> GetById(Guid id);
        public Task Create(Property property);
        public void Delete(Property property);
        public void Update(Property property);
    }
}
