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
    internal class UpdateAmenityRequestValidator : AbstractValidator<UpdateAmenityRequest>
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly Guid _amenityId;
        public UpdateAmenityRequestValidator(IRepositoryManager repositoryManager, Guid amenityId)
        {
            _repositoryManager = repositoryManager;
            _amenityId = amenityId;

            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(50)
                .MustAsync(async (name, cancellation) => await IsUniqueName(name))
                .WithMessage("Name must be unique");
        }

        private async Task<bool> IsUniqueName(string name)
        {
            var amenity = await _repositoryManager.Amenities.GetByName(name);
            if (amenity == null || amenity.Id == _amenityId)
            {
                return true;
            }
            return false;
        }
    }
}
