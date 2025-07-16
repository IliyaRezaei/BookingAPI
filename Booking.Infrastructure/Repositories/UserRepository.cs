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
    internal class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public UserRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Create(ApplicationUser user)
        {
            await _dbContext.Users.AddAsync(user);
        }

        public void Delete(ApplicationUser user)
        {
            _dbContext.Users.Remove(user);
        }

        public async Task<IEnumerable<ApplicationUser>> GetAll()
        {
            return await _dbContext.Users.ToListAsync();
        }

        public async Task<ApplicationUser?> GetByEmail(string email)
        {
            return await _dbContext.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<ApplicationUser?> GetById(Guid id)
        {
            return await _dbContext.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<ApplicationUser?> GetByUsername(string username)
        {
            return await _dbContext.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Username == username);
        }

        public void Update(ApplicationUser user)
        {
            _dbContext.Users.Update(user);
        }
    }
}
