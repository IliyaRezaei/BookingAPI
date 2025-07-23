using Booking.Domain.Abstractions.Repositories.Manager;
using Booking.Domain.Contracts.Booking;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.Validators.Booking
{
    internal class UpdateBookingRequestValidator : AbstractValidator<UpdateBookingRequest>
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly Guid _bookingId;
        private readonly Guid _propertyId;
        public UpdateBookingRequestValidator(IRepositoryManager repositoryManager, Guid bookingId, Guid propertyId)
        {
            _repositoryManager = repositoryManager;
            _bookingId = bookingId;
            _propertyId = propertyId;

            RuleFor(x => x.CheckIn)
                .Must(date => date >= DateOnly.FromDateTime(DateTime.UtcNow));

            RuleFor(x => x.CheckOut)
                .Must(date => date <= DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(1)));

            RuleFor(x => x)
                .MustAsync(async (dates, cancellation) => await IsBookingDateValid(dates.CheckIn, dates.CheckOut))
                .WithMessage("There is an interference with your booking date");
        }
        private async Task<bool> IsBookingDateValid(DateOnly checkInDate, DateOnly checkOutDate)
        {
            var daysBooked = (checkOutDate.ToDateTime(TimeOnly.MinValue) - checkInDate.ToDateTime(TimeOnly.MinValue)).Days;
            if (daysBooked < 1)
            {
                return false;
            }

            var allBookings = await _repositoryManager.Bookings.GetAllUpcomingOfAPropertyById(_propertyId);
            var bookings = allBookings.Where(b => b.Id != _bookingId).ToList();
            foreach (var booking in bookings)
            {
                var existingCheckIn = booking.CheckIn;
                var existingCheckOut = booking.CheckOut;
                bool isOverlappingIn = false;

                if (checkInDate < existingCheckIn)
                {
                    if (checkOutDate >= existingCheckIn)
                    {
                        isOverlappingIn = true;
                    }
                }
                if (checkInDate > existingCheckOut)
                {
                    isOverlappingIn = true;
                }

                if (isOverlappingIn)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
