using Booking.Domain.Abstractions.Repositories.Manager;
using Booking.Domain.Contracts.Amenity;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.Validators.Amenity
{
    public class CreateAmenityRequestValidator : AbstractValidator<CreateAmenityRequest>
    {
        private readonly IRepositoryManager _repositoryManager;
        public CreateAmenityRequestValidator(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;

            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(50)
                .MustAsync(async (name, cancellation) => await IsUniqueName(name))
                .WithMessage("Name must be unique");
        }

        private async Task<bool> IsUniqueName(string name)
        {
            var amenity = await _repositoryManager.Amenities.GetByName(name);
            return amenity == null;
        }
    }
}
