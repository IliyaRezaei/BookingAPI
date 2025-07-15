using Booking.Application.Errors;
using Booking.Application.Mappers;
using Booking.Application.Validators.Booking;
using Booking.Domain.Abstractions.Repositories.Manager;
using Booking.Domain.Abstractions.Services;
using Booking.Domain.Contracts.Booking;
using Booking.Domain.Errors;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.Services
{
    internal class BookingService : IBookingService
    {
        private readonly IRepositoryManager _repositoryManager;

        public BookingService(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }

        public async Task<BookingResponse> Create(CreateBookingRequest request, string username, Guid propertyId)
        {
            var validator = new CreateBookingRequestValidator(_repositoryManager, propertyId);
            await validator.ValidateAndThrowAsync(request);

            var client = await _repositoryManager.Users.GetByUsername(username);
            if (client == null) 
            {
                throw new UnauthorizedException("Login and try again");
            }
            if (client.IsHost)
            {
                var clientProperties = await _repositoryManager.Properties.GetAllPropertiesOfAUser(client);
                if (clientProperties.Any(p => p.Id == propertyId))
                {
                    throw new ForbiddenException("You can not book your own property");
                }
            }
            var property = await _repositoryManager.Properties.GetById(propertyId);
            if (property == null)
            {
                throw new NotFoundException($"Property with id {propertyId} not found");
            }

            int totalDays = (request.CheckIn.ToDateTime(TimeOnly.MinValue) - request.CheckOut.ToDateTime(TimeOnly.MinValue)).Days;
            int totalCost = property.PricePerNight * totalDays;
            var booking = request.ToEntity(client, property, totalCost);
            await _repositoryManager.Bookings.Create(booking);
            await _repositoryManager.SaveAsync();
            return booking.ToResponse();
        }

        public async Task Delete(Guid id, string username)
        {
            var user = await _repositoryManager.Users.GetByUsername(username);
            if (user == null)
            {
                throw new UnauthorizedException("Login and try again");
            }
            var userBookings = await _repositoryManager.Bookings.GetAllBookingsOfAUser(user);
            var booking = userBookings.Where(b => b.Id == id).FirstOrDefault();
            if (booking == null)
            {
                throw new NotFoundException($"Booking with id {id} not found");
            }
            _repositoryManager.Bookings.Delete(booking);
            await _repositoryManager.SaveAsync();
        }

        public async Task<IEnumerable<BookingResponse>> GetAll()
        {
            var bookings = await _repositoryManager.Bookings.GetAll();
            return bookings.ToResponse();
        }

        public async Task<BookingResponse> GetById(Guid id)
        {
            var booking = await _repositoryManager.Bookings.GetById(id);
            if(booking == null)
            {
                throw new NotFoundException("Booking not found");
            }
            return booking.ToResponse();
        }

        public async Task Update(UpdateBookingRequest request, Guid id, string username, Guid propertyId)
        {
            var validator = new UpdateBookingRequestValidator(_repositoryManager, id, propertyId);
            await validator.ValidateAndThrowAsync(request);

            var client = await _repositoryManager.Users.GetByUsername(username);
            if (client == null)
            {
                throw new UnauthorizedException("Login and try again");
            }
            var property = await _repositoryManager.Properties.GetById(propertyId);
            if (property == null)
            {
                throw new NotFoundException($"Property with id {propertyId} not found");
            }
            var clientBookings = await _repositoryManager.Bookings.GetAllBookingsOfAUser(client);
            var booking = clientBookings.FirstOrDefault(b => b.Id == id);
            if (booking == null)
            {
                throw new NotFoundException($"Booking with id {id} not found");
            }
            int totalDays = (request.CheckIn.ToDateTime(TimeOnly.MinValue) - request.CheckOut.ToDateTime(TimeOnly.MinValue)).Days;
            int totalCost = property.PricePerNight * totalDays;
            var entity = request.ToEntity(id,client, property, totalCost);
            _repositoryManager.Bookings.Update(entity);
            await _repositoryManager.SaveAsync();
        }
    }
}
