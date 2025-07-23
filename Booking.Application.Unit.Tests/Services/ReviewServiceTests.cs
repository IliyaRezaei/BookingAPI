using Booking.Application.Errors;
using Booking.Application.Mappers;
using Booking.Application.Services;
using Booking.Application.Utilities;
using Booking.Domain.Abstractions.Repositories.Manager;
using Booking.Domain.Contracts.Review;
using Booking.Domain.Contracts.Role;
using Booking.Domain.Entities;
using Booking.Domain.Errors;
using FluentValidation;
using NSubstitute;
using System.Xml.Linq;

namespace Booking.Application.Tests.Unit.Services
{
    public class ReviewServiceTests
    {
        private readonly IRepositoryManager _repository;
        private readonly ReviewService _service;
        public ReviewServiceTests()
        {
            _repository = Substitute.For<IRepositoryManager>();
            _service = new ReviewService(_repository);
            ExistingProperty = new Property
            {
                Id = Guid.NewGuid(),
                Title = "Title",
                Description = "Description",
                BedRooms = 1,
                Beds = 1,
                Bathrooms = 1,
                Guests = 1,
            };
            ExistingClient = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Username = "Username",
                NormalizedUsername = "USERNAME",
                Email = "Email@gmail.com",
                NormalizedEmail = "EMAIL@GMAIL.COM",
                HashedPassword = "Password1234@".HashPassword(),
                ImageUrl = "",
                IsHost = false,
            };
            ExistingReview = new Review
            {
                Id = Guid.NewGuid(),
                Client = ExistingClient,
                Property = ExistingProperty,
                Comment = "Comment on ExistingProperty",
                Rating = 1
            };
            AllReviews = new List<Review>
            {
                new Review
                {
                    Id = Guid.NewGuid(),
                    Client = ExistingClient,
                    Property = ExistingProperty,
                    Comment = "Comment on ExistingProperty1",
                    Rating = 2
                },new Review
                {
                    Id = Guid.NewGuid(),
                    Client = ExistingClient,
                    Property = ExistingProperty,
                    Comment = "Comment on ExistingProperty2",
                    Rating = 3
                },new Review
                {
                    Id = Guid.NewGuid(),
                    Client = ExistingClient,
                    Property = ExistingProperty,
                    Comment = "Comment on ExistingProperty3",
                    Rating = 4
                },
            };
        }

        [Theory]
        [InlineData("ThisIsValidComment", 5)]
        [InlineData("ThisIsValidComment", 1)]
        public async Task Create_ShouldReturnReviewResponse_WhenValidRequest(string comment, int rating)
        {
            //Arrange
            var request = new CreateReviewRequest
            {
                Comment = comment,
                Rating = rating
            };
            _repository.Users.GetByUsername(ExistingClient.Username).Returns(ExistingClient);
            _repository.Properties.GetById(ExistingProperty.Id).Returns(ExistingProperty);

            //Act
            var response = await _service.Create(request, ExistingClient.Username, ExistingProperty.Id);

            //Assert
            Assert.NotNull(response);
            Assert.IsType<ReviewResponse>(response);
            Assert.Equal(rating, response.Rating);
            Assert.Equal(comment, response.Comment);
        }

        [Theory]
        [InlineData("ThisIsValidComment",10)]
        [InlineData("Invalid", 1)]
        public async Task Create_ShouldThrowValidationException_WhenInvalidRequest(string comment, int rating)
        {
            //Arrange
            var request = new CreateReviewRequest
            {
                Comment = comment,
                Rating = rating
            };
            _repository.Users.GetByUsername(ExistingClient.Username).Returns(Task.FromResult<ApplicationUser>(null));
            _repository.Properties.GetById(ExistingProperty.Id).Returns(Task.FromResult<Property>(null));

            //Act
            var exception = await Assert.ThrowsAsync<ValidationException>(async () => await _service.Create(request, ExistingClient.Username, ExistingProperty.Id));

            //Assert
            Assert.NotNull(exception);
            Assert.IsType<ValidationException>(exception);
        }

        [Fact]
        public async Task Create_ShouldThrowNotFoundException_WhenPropertyDoesntExist()
        {
            //Arrange
            var request = new CreateReviewRequest
            {
                Comment = "comment on property",
                Rating = 5
            };
            _repository.Users.GetByUsername(ExistingClient.Username).Returns(ExistingClient);
            _repository.Properties.GetById(ExistingProperty.Id).Returns(Task.FromResult<Property>(null));

            //Act
            var exception = await Assert.ThrowsAsync<NotFoundException>(async () => await _service.Create(request, ExistingClient.Username, ExistingProperty.Id));

            //Assert
            Assert.NotNull(exception);
            Assert.IsType<NotFoundException>(exception);
        }

        [Fact]
        public async Task Create_ShouldThrowForbiddenException_WhenYouTryToReviewAPropertyMultipleTimes()
        {
            //Arrange
            var request = new CreateReviewRequest
            {
                Comment = "comment on property",
                Rating = 5
            };
            var existingReview = new Review
            {
                Id = Guid.NewGuid(),
                Client = ExistingClient,
                Property = ExistingProperty,
                Comment = "existing comment on property",
                Rating = 4
            };
            _repository.Users.GetByUsername(ExistingClient.Username).Returns(ExistingClient);
            _repository.Properties.GetById(ExistingProperty.Id).Returns(ExistingProperty);
            _repository.Reviews.GetAllReviewsOfAUser(ExistingClient).Returns(new List<Review> { existingReview });

            //Act
            var exception = await Assert.ThrowsAsync<ForbiddenException>(async () => await _service.Create(request, ExistingClient.Username, ExistingProperty.Id));

            //Assert
            Assert.NotNull(exception);
            Assert.IsType<ForbiddenException>(exception);
        }

        [Fact]
        public async Task Update_ShouldUpdate_WhenValidRequest()
        {
            //Arrange
            var request = new UpdateReviewRequest
            {
                Comment = "New Comment On Property",
                Rating = 5
            };
            _repository.Users.GetByUsername(ExistingClient.Username).Returns(ExistingClient);
            _repository.Properties.GetById(ExistingProperty.Id).Returns(ExistingProperty);
            _repository.Reviews.GetAllReviewsOfAUser(ExistingClient).Returns(new List<Review> { ExistingReview });

            //Act
            await _service.Update(request, ExistingReview.Id, ExistingClient.Username, ExistingProperty.Id);

            //Assert
            _repository.Users.Received(1).GetByUsername(ExistingClient.Username);
            _repository.Properties.Received(1).GetById(ExistingProperty.Id);
            _repository.Reviews.Received(1).GetAllReviewsOfAUser(ExistingClient);
            _repository.Reviews.Received(1).Update(Arg.Any<Review>());
            _repository.Received(1).SaveAsync();
        }

        [Fact]
        public async Task Update_ShouldThrowValidationException_WhenUpdateReviewRequestIsInvalid()
        {
            //Arrange
            var request = new UpdateReviewRequest
            {
                Comment = "invalid",
                Rating = 10
            };

            //Act
            var exception = await Assert.ThrowsAsync<ValidationException>(async () =>
            await _service.Update(request, ExistingReview.Id, ExistingClient.Username, ExistingProperty.Id));

            //Assert
            Assert.NotNull(exception);
            Assert.IsType<ValidationException>(exception);
        }

        [Fact]
        public async Task Update_ShouldThrowNotFoundException_WhenNoReviewWithReviewIdExist()
        {
            //Arrange
            var nonExistingReviewId = Guid.NewGuid();
            var request = new UpdateReviewRequest
            {
                Comment = "Valid Comment",
                Rating = 5
            };
            _repository.Users.GetByUsername(ExistingClient.Username).Returns(ExistingClient);
            _repository.Properties.GetById(ExistingProperty.Id).Returns(ExistingProperty);

            //Act
            var exception = await Assert.ThrowsAsync<NotFoundException>(async () => 
            await _service.Update(request, nonExistingReviewId, ExistingClient.Username, ExistingProperty.Id));

            //Assert
            Assert.NotNull(exception);
            Assert.IsType<NotFoundException>(exception);
            Assert.Contains(nonExistingReviewId.ToString(), exception.ErrorMessage);
        }
        [Fact]
        public async Task Update_ShouldThrowNotFoundException_WhenNoPropertyWithPropertyIdExist()
        {
            //Arrange
            var nonExistingPropertyId = Guid.NewGuid();
            var request = new UpdateReviewRequest
            {
                Comment = "Valid Comment",
                Rating = 5
            };
            _repository.Users.GetByUsername(ExistingClient.Username).Returns(ExistingClient);
            _repository.Reviews.GetAllReviewsOfAUser(ExistingClient).Returns(new List<Review> { ExistingReview });

            //Act
            var exception = await Assert.ThrowsAsync<NotFoundException>(async () =>
            await _service.Update(request, ExistingReview.Id, ExistingClient.Username, nonExistingPropertyId));

            //Assert
            Assert.NotNull(exception);
            Assert.IsType<NotFoundException>(exception);
            Assert.Contains(nonExistingPropertyId.ToString(), exception.ErrorMessage);
        }
        [Fact]
        public async Task DeleteById_ShouldDeleteClientReview_WhenReviewIsClientsReview()
        {
            //Arrange
            var reviewId = ExistingReview.Id;
            _repository.Users.GetByUsername(ExistingClient.Username).Returns(ExistingClient);
            _repository.Reviews.GetAllReviewsOfAUser(ExistingClient).Returns(new List<Review> { ExistingReview });

            //Act
            await _service.Delete(reviewId, ExistingClient.Username);

            //Assert
            await _repository.Users.Received(1).GetByUsername(ExistingClient.Username);
            await _repository.Reviews.Received(1).GetAllReviewsOfAUser(ExistingClient);
            _repository.Reviews.Received(1).Delete(
                Arg.Is<Review>(
                    e => e.Id == reviewId)
                );
            await _repository.Received(1).SaveAsync();
        }

        [Fact]
        public async Task DeleteById_ShouldThrowNotFoundException_WhenReviewIsNotClientsReview()
        {
            //Arrange
            var reviewId = ExistingReview.Id;
            _repository.Users.GetByUsername(ExistingClient.Username).Returns(ExistingClient);
            _repository.Reviews.GetAllReviewsOfAUser(ExistingClient).Returns(Task.FromResult<IEnumerable<Review>>(new List<Review>()));

            //Act
            var exception = await Assert.ThrowsAsync<NotFoundException>(async () =>
            await _service.Delete(reviewId, ExistingClient.Username));

            //Assert
            await _repository.Users.Received(1).GetByUsername(ExistingClient.Username);
            await _repository.Reviews.Received(1).GetAllReviewsOfAUser(ExistingClient);

            Assert.NotNull(exception);
            Assert.IsType<NotFoundException>(exception);
            Assert.Contains(reviewId.ToString(), exception.ErrorMessage);
        }

        [Fact]
        public async Task DeleteById_ShouldThrowNotFoundException_WhenUsernameDoesntBelongToAnyone()
        {
            //Arrange
            _repository.Users.GetByUsername(ExistingClient.Username).Returns(Task.FromResult<ApplicationUser>(null));
            _repository.Reviews.GetAllReviewsOfAUser(ExistingClient).Returns(new List<Review> { ExistingReview });

            //Act
            var exception = await Assert.ThrowsAsync<UnauthorizedException>(async () =>
            await _service.Delete(ExistingReview.Id, ExistingClient.Username));

            //Assert
            await _repository.Users.Received(1).GetByUsername(ExistingClient.Username);

            Assert.NotNull(exception);
            Assert.IsType<UnauthorizedException>(exception);
        }

        [Fact]
        public async Task GetById_ShouldReturnReviewAsReviewResponse_WhenReviewIdIsValid()
        {
            //Arrange
            var reviewId = ExistingReview.Id;
            _repository.Reviews.GetById(reviewId).Returns(ExistingReview);

            //Act
            var response = await _service.GetById(reviewId);

            //Assert
            Assert.NotNull(response);
            Assert.IsType<ReviewResponse>(response);
            Assert.Equal(ExistingReview.Id, response.Id);
            Assert.Equal(ExistingReview.Comment, response.Comment);
            Assert.Equal(ExistingReview.Rating, response.Rating);
        }

        [Fact]
        public async Task GetById_ShouldThrowNotFoundException_WhenReviewIdIsInvalid()
        {
            //Arrange
            var reviewId = Guid.NewGuid();
            _repository.Reviews.GetById(reviewId).Returns(Task.FromResult<Review>(null));

            //Act
            var exception = await Assert.ThrowsAsync<NotFoundException>(async () =>
            await _service.GetById(reviewId));

            //Assert
            Assert.NotNull(exception);
            Assert.IsType<NotFoundException>(exception);
            Assert.Contains(reviewId.ToString(), exception.ErrorMessage);
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllReviewsAsReviewResponses_EvenIfReviewsDontExist()
        {
            //Arrange
            _repository.Reviews.GetAll().Returns(AllReviews);

            //Act
            var reviewResponses = await _service.GetAll();

            //Assert
            Assert.NotNull(reviewResponses);
            Assert.IsType<List<ReviewResponse>>(reviewResponses);
            Assert.All(reviewResponses, review => Assert.NotNull(review.Comment));
        }

        private ApplicationUser ExistingClient { get; set; }
        private Property ExistingProperty { get; set; }
        private Review ExistingReview { get; set; }
        private List<Review> AllReviews { get; set; }
    }
}
