using Booking.Domain.Abstractions.Repositories.Manager;
using Booking.Domain.Contracts.Property;
using Booking.Domain.Errors;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.Validators.Property
{
    internal class CreatePropertyRequestValidator : AbstractValidator<CreatePropertyRequest>
    {
        private readonly IRepositoryManager _repositoryManager;
        public CreatePropertyRequestValidator(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;

            RuleFor(x => x.Title)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.Description)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.PricePerNight)
                .NotEmpty()
                .GreaterThanOrEqualTo(10);

            RuleFor(x => x.Guests)
                .NotEmpty()
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.BedRooms)
                .NotEmpty()
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.Bathrooms)
                .NotEmpty()
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.Beds)
                .NotEmpty()
                .GreaterThanOrEqualTo(0);
        }
    }
}
