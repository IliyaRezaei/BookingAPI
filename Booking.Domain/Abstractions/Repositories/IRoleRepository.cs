using Booking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Domain.Abstractions.Repositories
{
    public interface IRoleRepository
    {
        public Task<IEnumerable<ApplicationRole>> GetAll();
        public Task<ApplicationRole?> GetById(Guid id);
        public Task<ApplicationRole?> GetByName(string name);
        public Task Create(ApplicationRole role);
        public void Delete(ApplicationRole role);
        public void Update(ApplicationRole role);
    }
}
