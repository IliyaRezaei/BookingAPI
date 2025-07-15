using Booking.Domain.Contracts.Property;
using Booking.Domain.Contracts.Review;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Domain.Abstractions.Services
{
    public interface IReviewService
    {
        public Task<IEnumerable<ReviewResponse>> GetAll();
        public Task<ReviewResponse> GetById(Guid id);
        public Task<ReviewResponse> Create(CreateReviewRequest request, string username, Guid propertyId);
        public Task Delete(Guid id);
        public Task Update(UpdateReviewRequest request, Guid id, string username, Guid propertyId);
    }
}
