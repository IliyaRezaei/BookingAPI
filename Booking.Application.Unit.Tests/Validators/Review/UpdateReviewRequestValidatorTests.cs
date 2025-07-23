using Booking.Application.Validators.Review;
using Booking.Domain.Contracts.Review;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.Tests.Unit.Validators.Review
{
    public class UpdateReviewRequestValidatorTests
    {
        private readonly UpdateReviewRequestValidator _validator;
        public UpdateReviewRequestValidatorTests()
        {
            _validator = new UpdateReviewRequestValidator();
        }

        [Theory]
        [InlineData("valid comment")]
        public async Task ShouldPass_WhenCommentIsValid(string comment)
        {
            //Arrange
            var request = new UpdateReviewRequest
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
            var request = new UpdateReviewRequest
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
            var request = new UpdateReviewRequest
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
            var request = new UpdateReviewRequest
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
