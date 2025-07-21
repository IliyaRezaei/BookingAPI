using Booking.Application.Mappers;
using Booking.Application.Utilities;
using Booking.Domain.Contracts.Role;
using Booking.Domain.Contracts.User;
using Booking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.Tests.Unit.Mappers
{
    public class UserMapperTests
    {
        [Fact]
        public void ToEntity_ShouldReturnValidUserEntity_FromUserRegisterRequest()
        {
            //Arrange
            var request = new UserRegisterRequest 
            { 
                Email = "Email@gmail.com",
                Username = "Username", 
                Password = "Password1234@",
                ConfirmPassword = "Password1234@"
            };

            //Act
            var entity = request.ToEntity();

            //Assert
            Assert.NotNull(entity);
            Assert.IsType<ApplicationUser>(entity);
            Assert.IsType<Guid>(entity.Id);
            Assert.NotNull(entity.Email);
            Assert.Equal(request.Email, entity.Email);
            Assert.Equal(request.Email.ToUpper(), entity.NormalizedEmail);
            Assert.NotNull(entity.Username);
            Assert.Equal(request.Username, entity.Username);
            Assert.Equal(request.Username.ToUpper(), entity.NormalizedUsername);
            Assert.True(AuthExtensions.VerifyPassword(request.Password, entity.HashedPassword));
        }

        [Fact]
        public void ToResponse_ShouldReturnValidUserResponse_FromAnUserEntity()
        {
            //Arrange
            var entity = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Username = "Username",
                NormalizedUsername = "USERNAME",
                Email = "Email@gmail.com",
                NormalizedEmail = "EMAIL@GMAIL.COM",
                IsHost = true,
            };

            //Act
            var response = entity.ToResponse();

            //Assert
            Assert.NotNull(response);
            Assert.IsType<UserResponse>(response);
            Assert.Equal(entity.Id, response.Id);
            Assert.Equal(entity.Username, response.Username);
            Assert.Equal(entity.Email, response.Email);
            Assert.Equal(entity.IsHost, response.IsHost);
        }

        [Fact]
        public void ToResponse_ShouldReturnValidUserResponses_FromListOfUserEntities()
        {
            //Arrange
            var entities = new List<ApplicationUser>
            {
                new ApplicationUser 
                {
                    Id = Guid.NewGuid(),
                    Username = "Username1",
                    NormalizedUsername = "USERNAME1",
                    Email = "Email1@gmail.com",
                    NormalizedEmail = "EMAIL1@GMAIL.COM",
                    IsHost = true,
                },
                new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    Username = "Username2",
                    NormalizedUsername = "USERNAME2",
                    Email = "Email2@gmail.com",
                    NormalizedEmail = "EMAIL2@GMAIL.COM",
                    IsHost = false,
                },
            };

            //Act
            var responses = entities.ToResponse();

            //Assert
            Assert.NotNull(responses);
            Assert.IsType<List<UserResponse>>(responses);
            Assert.All(entities, entity => Assert.Contains(responses, user => user.Id == entity.Id));
            Assert.All(entities, entity => Assert.Contains(responses, user => user.Username == entity.Username));
            Assert.All(entities, entity => Assert.Contains(responses, user => user.Email == entity.Email));
            Assert.All(entities, entity => Assert.Contains(responses, user => user.IsHost == entity.IsHost));
        }
    }
}
