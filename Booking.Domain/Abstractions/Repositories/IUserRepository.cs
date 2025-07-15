using Booking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Domain.Abstractions.Repositories
{
    public interface IUserRepository
    {
        public Task<IEnumerable<ApplicationUser>> GetAll();
        public Task<ApplicationUser?> GetById(Guid id);
        public Task<ApplicationUser?> GetByUsername(string username);
        public Task<ApplicationUser?> GetByEmail(string email);
        public Task Create(ApplicationUser user);
        public void Delete(ApplicationUser user);
        public void Update(ApplicationUser user);
    }
}
