using Booking.Application.Errors;
using Booking.Application.Mappers;
using Booking.Application.Validators.Review;
using Booking.Domain.Abstractions.Repositories.Manager;
using Booking.Domain.Abstractions.Services;
using Booking.Domain.Contracts.Review;
using Booking.Domain.Errors;
using FluentValidation;
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
            var validator = new CreateReviewRequestValidator();
            validator.ValidateAndThrow(request);

            var client = await _repositoryManager.Users.GetByUsername(username) ?? 
                throw new UnauthorizedException("Login and try again");

            var property = await _repositoryManager.Properties.GetById(propertyId) ??
                throw new NotFoundException($"Property with id {propertyId} not found");

            var clientReviews = await _repositoryManager.Reviews.GetAllReviewsOfAUser(client);
            if (clientReviews.Any(x => x.Property == property))
            {
                throw new ForbiddenException("You are not allowed to comment multiple times on a single property");
            }
            var review = request.ToEntity(client, property);
            await _repositoryManager.Reviews.Create(review);
            await _repositoryManager.SaveAsync();
            return review.ToResponse();
        }

        public async Task Delete(Guid id, string username)
        {
            var client = await _repositoryManager.Users.GetByUsername(username) ??
                throw new UnauthorizedException("Login and try again");
            var clientReviews = await _repositoryManager.Reviews.GetAllReviewsOfAUser(client);
            var review = clientReviews.FirstOrDefault(x => x.Id == id);
            if (review == null)
            {
                throw new NotFoundException($"Review with id {id} not found");
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
                throw new NotFoundException($"Reviw with id {id} not found");
            }
            return review.ToResponse();
        }

        public async Task Update(UpdateReviewRequest request,Guid id , string username, Guid propertyId)
        {
            var validator = new UpdateReviewRequestValidator();
            validator.ValidateAndThrow(request);

            var client = await _repositoryManager.Users.GetByUsername(username) ??
                throw new UnauthorizedException("Login and try again");

            var property = await _repositoryManager.Properties.GetById(propertyId) ??
                throw new NotFoundException($"Property with id {propertyId} not found");

            var clientReviews = await _repositoryManager.Reviews.GetAllReviewsOfAUser(client);
            var review = clientReviews.FirstOrDefault(x => x.Id == id);
            if (review == null)
            {
                throw new NotFoundException($"Review with id {id} not found");
            }

            var entity = request.ToEntity(id, client, property);
            _repositoryManager.Reviews.Update(entity);
            await _repositoryManager.SaveAsync();
        }
    }
}
