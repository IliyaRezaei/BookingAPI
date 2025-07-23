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
    internal class CreateBookingRequestValidator : AbstractValidator<CreateBookingRequest>
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly Guid _propertyId;
        public CreateBookingRequestValidator(IRepositoryManager repositoryManager, Guid propertyId)
        {
            _repositoryManager = repositoryManager;
            _propertyId = propertyId;

            RuleFor(x => x.CheckIn)
                .Must(date => date >= DateOnly.FromDateTime(DateTime.UtcNow));

            RuleFor(x => x.CheckOut)
                .Must(date => date <= DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(1)));

            RuleFor(x => x)
                .MustAsync(async (dates, cancellation) => await IsBookingDateValid(dates.CheckIn, dates.CheckOut))
                .WithMessage("interefere with other bookings or bad bookig date");
        }

        private async Task<bool> IsBookingDateValid(DateOnly checkInDate, DateOnly checkOutDate)
        {
            var daysBooked = (checkOutDate.ToDateTime(TimeOnly.MinValue) - checkInDate.ToDateTime(TimeOnly.MinValue)).Days + 1;
            if (daysBooked < 1)
            {
                return false;
            }

            var bookings = await _repositoryManager.Bookings.GetAllUpcomingOfAPropertyById(_propertyId);
            foreach (var booking in bookings)
            {
                var existingCheckIn = booking.CheckIn;
                var existingCheckOut = booking.CheckOut;

                if (checkInDate <= existingCheckOut && checkOutDate >= existingCheckIn)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
