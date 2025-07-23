using Booking.Domain.Contracts.Review;
using Booking.Domain.Entities;

namespace Booking.Application.Mappers
{
    internal static class ReviewMappers
    {
        public static Review ToEntity(this CreateReviewRequest request, ApplicationUser client, Property property)
        {
            return new Review
            {
                Id = Guid.NewGuid(),
                Client = client,
                Comment = request.Comment,
                Rating = request.Rating,
                Property = property,
            };
        }

        public static Review ToEntity(this UpdateReviewRequest request, Guid reviewId, ApplicationUser client, Property property)
        {
            return new Review
            {
                Id = reviewId,
                Client = client,
                Comment = request.Comment,
                Rating = request.Rating,
                Property = property,
            };
        }

        public static ReviewResponse ToResponse(this Review review)
        {
            return new ReviewResponse
            {
                Id = review.Id,
                Comment = review.Comment,
                Rating = review.Rating,
            };
        }

        public static IEnumerable<ReviewResponse> ToResponse(this IEnumerable<Review> reviews)
        {
            return reviews.Select(r => r.ToResponse()).ToList();
        }
    }
}
