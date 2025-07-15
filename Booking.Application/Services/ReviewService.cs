using Booking.Application.Mappers;
using Booking.Domain.Abstractions.Repositories.Manager;
using Booking.Domain.Abstractions.Services;
using Booking.Domain.Contracts.Review;
using Booking.Domain.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IRepositoryManager _repositoryManager;

        public ReviewService(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }

        public async Task<ReviewResponse> Create(CreateReviewRequest request, string username, Guid propertyId)
        {
            var client = await _repositoryManager.Users.GetByUsername(username);
            var property = await _repositoryManager.Properties.GetById(propertyId);
            if (property == null || client == null)
            {
                throw new Exception("Bad Request");
            }
            var review = request.ToEntity(client, property);
            await _repositoryManager.Reviews.Create(review);
            await _repositoryManager.SaveAsync();
            return review.ToResponse();
        }

        public async Task Delete(Guid id)
        {
            var review = await _repositoryManager.Reviews.GetById(id);
            if (review == null)
            {
                throw new Exception("Review not found");
            }
            _repositoryManager.Reviews.Delete(review);
            await _repositoryManager.SaveAsync();
        }

        public async Task<IEnumerable<ReviewResponse>> GetAll()
        {
            var reviews = await _repositoryManager.Reviews.GetAll();
            return reviews.ToResponse();
        }

        public async Task<ReviewResponse> GetById(Guid id)
        {
            var review = await _repositoryManager.Reviews.GetById(id);
            if(review == null)
            {
                throw new Exception("Reviw not found");
            }
            return review.ToResponse();
        }

        public async Task Update(UpdateReviewRequest request,Guid id , string username, Guid propertyId)
        {

            var review = await _repositoryManager.Reviews.GetById(id);
            if (review == null)
            {
                throw new NotFoundException($"Review with id {id} not found");
            }

            var client = await _repositoryManager.Users.GetByUsername(username);
            var property = await _repositoryManager.Properties.GetById(propertyId);
            if (property == null || client == null)
            {
                throw new Exception("Bad Request");
            }
            var entity = request.ToEntity(id, client, property);
            _repositoryManager.Reviews.Update(entity);
            await _repositoryManager.SaveAsync();
        }
    }
}
