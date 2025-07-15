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
    internal class ReviewRepository : IReviewRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ReviewRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Create(Review review)
        {
            await _dbContext.Reviews.AddAsync(review);
        }

        public void Delete(Review review)
        {
            _dbContext.Reviews.Remove(review);
        }

        public async Task<IEnumerable<Review>> GetAll()
        {
            return await _dbContext.Reviews.ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetAllReviewsOfAUser(ApplicationUser user)
        {
            return await _dbContext.Reviews.Where(r => r.Client == user).ToListAsync();
        }

        public async Task<Review?> GetById(Guid id)
        {
            return await _dbContext.Reviews.FindAsync(id);
        }

        public void Update(Review review)
        {
            _dbContext.Reviews.Update(review);
        }
    }
}
