using Booking.Application.Mappers;
using Booking.Application.Utilities;
using Booking.Domain.Contracts.Review;
using Booking.Domain.Contracts.Role;
using Booking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.Tests.Unit.Mappers
{
    public class ReviewMapperTests
    {
        public ReviewMapperTests()
        {
            ExistingClient = new ApplicationUser 
            { 
                Id = Guid.NewGuid(), 
                Username = "Username", 
                NormalizedUsername = "USERNAME",
                Email = "Email@gmail.com",
                NormalizedEmail = "EMAIL@GMAIL.COM",
                HashedPassword = "Test1234@".HashPassword(),
                ImageUrl = "",
                IsHost = false
            };
            ExistingProperty = new Property
            {
                Id = Guid.NewGuid(),
                Title = "Title",
                Description = "Description",
                PricePerNight = 1000,
                Bathrooms = 1,
                Beds = 1,
                Guests = 1,
                BedRooms = 1,
            };
        }

        [Fact]
        public void ToEntity_ShouldReturnValidReviewEntity_FromCreateReviewRequest()
        {
            //Arrange
            var request = new CreateReviewRequest { Comment = "Comment on property", Rating = 4 };

            //Act
            var entity = request.ToEntity(ExistingClient, ExistingProperty);

            //Assert
            Assert.NotNull(entity);
            Assert.IsType<Review>(entity);
            Assert.IsType<Guid>(entity.Id);
            Assert.Equal(request.Comment, entity.Comment);
            Assert.NotNull(entity.Comment);
            Assert.Equal(request.Rating, entity.Rating);
            Assert.Equal(entity.Client, ExistingClient);
            Assert.Equal(entity.Property, ExistingProperty);
        }

        [Fact]
        public void ToEntity_ShouldReturnValidReviewEntity_FromUpdateReviewRequest()
        {
            //Arrange
            var reviewId = Guid.NewGuid();
            var request = new UpdateReviewRequest
            {
                Comment = "new Comment on property",
                Rating = 5
            };

            //Act
            var entity = request.ToEntity(reviewId, ExistingClient, ExistingProperty);

            //Assert
            Assert.NotNull(entity);
            Assert.IsType<Review>(entity);
            Assert.IsType<Guid>(entity.Id);
            Assert.Equal(request.Comment, entity.Comment);
            Assert.NotNull(entity.Comment);
            Assert.Equal(request.Rating, entity.Rating);
            Assert.Equal(entity.Client, ExistingClient);
            Assert.Equal(entity.Property, ExistingProperty);
        }

        [Fact]
        public void ToResponse_ShouldReturnValidReviewResponse_FromReviewEntity()
        {
            //Arrange
            var reviewId = Guid.NewGuid();
            var entity = new Review
            {
                Id = reviewId,
                Comment = "Comment on property",
                Rating = 5,
                Client = ExistingClient,
                Property = ExistingProperty
            };

            //Act
            var response = entity.ToResponse();

            //Assert
            Assert.NotNull(response);
            Assert.IsType<ReviewResponse>(response);
            Assert.Equal(entity.Id, response.Id);
            Assert.Equal(entity.Comment, response.Comment);
            Assert.Equal(entity.Rating, response.Rating);
        }

        [Fact]
        public void ToResponse_ShouldReturnValidReviewResponses_FromReviewEntities()
        {
            //Arrange
            var entities = new List<Review>
            {
                new Review 
                { 
                    Id = Guid.NewGuid(), 
                    Comment = "Random1", 
                    Rating = 5, 
                    Client = ExistingClient, 
                    Property = ExistingProperty 
                },new Review
                {
                    Id = Guid.NewGuid(),
                    Comment = "Random2",
                    Rating = 4, 
                    Client = ExistingClient,
                    Property = ExistingProperty
                },new Review
                {
                    Id = Guid.NewGuid(),
                    Comment = "Random3",
                    Rating = 3, 
                    Client = ExistingClient,
                    Property = ExistingProperty
                },
            };

            //Act
            var responses = entities.ToResponse();

            //Assert
            Assert.NotNull(responses);
            Assert.IsType<List<ReviewResponse>>(responses);
            Assert.All(entities, entity => Assert.Contains(responses, review => review.Id == entity.Id));
            Assert.All(entities, entity => Assert.Contains(responses, review => review.Comment == entity.Comment));
            Assert.All(entities, entity => Assert.Contains(responses, review => review.Rating == entity.Rating));
        }


        private ApplicationUser ExistingClient { get; }
        private Property ExistingProperty { get; }
    }
}
