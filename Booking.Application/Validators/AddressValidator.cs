using Booking.Domain.ValueObjects;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.Validators
{
    internal class AddressValidator : AbstractValidator<Address>
    {
        public AddressValidator()
        {
            RuleFor(x => x.LocationDescription)
                .NotEmpty().WithMessage("Location Description is required")
                .Length(10,100).WithMessage("Location Description must be 10 to 100 characters");

            RuleFor(x => x.Latitude)
                .NotEmpty().WithMessage("Latitude is required")
                .ExclusiveBetween(-90, 90).WithMessage("Latitude is not valid");

            RuleFor(x => x.Longitude)
                .NotEmpty().WithMessage("Longitude is required")
                .ExclusiveBetween(-180, 180).WithMessage("Longitude is not valid");
        }
    }
}
