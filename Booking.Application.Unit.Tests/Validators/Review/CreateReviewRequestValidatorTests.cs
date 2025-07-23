using Booking.Application.Utilities;
using Booking.Application.Validators.Booking;
using Booking.Application.Validators.Review;
using Booking.Domain.Abstractions.Repositories.Manager;
using Booking.Domain.Contracts.Review;
using Booking.Domain.Entities;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.Tests.Unit.Validators.Review
{
    public class CreateReviewRequestValidatorTests
    {
        private readonly CreateReviewRequestValidator _validator;
        public CreateReviewRequestValidatorTests()
        {
            _validator = new CreateReviewRequestValidator();
        }

        [Theory]
        [InlineData("valid comment")]
        public async Task ShouldPass_WhenCommentIsValid(string comment)
        {
            //Arrange
            var request = new CreateReviewRequest
            {
                Comment = comment,
                Rating = 5
            };
            //Act
            var result = _validator.Validate(request);
            //Assert
            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public void ShouldPass_WhenRatingIsValid(int rating)
        {
            //Arrange
            var request = new CreateReviewRequest
            {
                Comment = "valid comment",
                Rating = rating
            };
            //Act
            var result = _validator.Validate(request);
            //Assert
            Assert.True(result.IsValid);
        }

        [Theory]
        [InlineData("invalid")]
        public void ShouldFail_WhenCommentIsInvalid(string comment)
        {
            //Arrange
            var request = new CreateReviewRequest
            {
                Comment = comment,
                Rating = 5
            };
            //Act
            var result = _validator.Validate(request);
            //Assert
            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        public void ShouldFail_WhenRatingIsInvalid(int rating)
        {
            //Arrange
            var request = new CreateReviewRequest
            {
                Comment = "valid comment",
                Rating = rating
            };
            //Act
            var result = _validator.Validate(request);
            //Assert
            Assert.False(result.IsValid);
        }
    }
}
