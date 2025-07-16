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
    internal class RoleRepository : IRoleRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public RoleRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Create(ApplicationRole role)
        {
            await _dbContext.Roles.AddAsync(role);
        }

        public void Delete(ApplicationRole role)
        {
            _dbContext.Roles.Remove(role);
        }

        public async Task<IEnumerable<ApplicationRole>> GetAll()
        {
            return await _dbContext.Roles.ToListAsync();
        }

        public async Task<ApplicationRole?> GetById(Guid id)
        {
            return await _dbContext.Roles.FindAsync(id);
        }

        public async Task<ApplicationRole?> GetByName(string name)
        {
            return await _dbContext.Roles
                .FirstOrDefaultAsync(r => r.NormalizedName == name.ToUpper());
        }

        public void Update(ApplicationRole role)
        {
            _dbContext.Roles.Update(role);
        }
    }
}
