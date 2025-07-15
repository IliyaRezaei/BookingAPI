using Booking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Domain.Abstractions.Repositories
{
    public interface IReviewRepository
    {
        public Task<IEnumerable<Review>> GetAll();
        public Task<IEnumerable<Review>> GetAllReviewsOfAUser(ApplicationUser user);
        public Task<Review?> GetById(Guid id);
        public Task Create(Review review);
        public void Delete(Review review);
        public void Update(Review review);
    }
}
