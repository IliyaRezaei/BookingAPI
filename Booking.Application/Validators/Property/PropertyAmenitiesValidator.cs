using Booking.Domain.Abstractions.Repositories.Manager;
using Booking.Domain.Contracts.Property;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.Validators.Property
{
    internal class PropertyAmenitiesValidator : AbstractValidator<PropertyAmenitiesRequest>
    {
        private readonly IRepositoryManager _repositoryManager;
        public PropertyAmenitiesValidator(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;

            RuleForEach(x => x.Amenities)
                .MustAsync(async (amenityId, cancellation) => await AmenityExistById(amenityId))
                .WithMessage(amenityId => $"Amenity with id {amenityId} does not exist");

        }

        private async Task<bool> AmenityExistById(Guid amenityId)
        {
            var amenity = await _repositoryManager.Amenities.GetById(amenityId);
            return amenity != null;
        }
    }
}
