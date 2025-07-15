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
    internal class PropertyAddressValidator : AbstractValidator<PropertyAddressRequest>
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly Guid _propertyId;
        private readonly string _username;
        public PropertyAddressValidator(IRepositoryManager repositoryManager, Guid propertyId, string username)
        {
            _repositoryManager = repositoryManager;
            _propertyId = propertyId;
            _username = username;

            RuleFor(x => x.CityId)
                .NotEmpty()
                .MustAsync(async (cityId, cancellation) => await CityExistsById(cityId))
                .WithMessage(cityId => $"City with id {cityId} does not exist");

            RuleFor(x => x.LocationDescription)
                .NotEmpty()
                .MaximumLength(250);

            RuleFor(x => x.Latitude)
                .NotEmpty()
                .InclusiveBetween(-90, 90);

            RuleFor(x => x.Longitude)
                .NotEmpty()
                .InclusiveBetween(-180, 180);

            RuleFor(x => x)
                .CustomAsync(async (request, context, cancellation) =>
                {
                    var property = await _repositoryManager.Properties.GetById(propertyId);
                    if (property == null)
                    {
                        throw new NotFoundException($"Property with id {propertyId} not found");
                    }
                    var host = await _repositoryManager.Users.GetByUsername(username);
                    if(host == null)
                    {
                        throw new NotFoundException($"User with name {username} not found");
                    }
                    if (property.Host.Username != host.Username)
                    {
                        context.AddFailure("IsHost", "You dont own this property");
                    }
                });
        }

        private async Task<bool> CityExistsById(Guid cityId)
        {
            var country = await _repositoryManager.Countries.GetById(cityId);
            return country != null;
        }
    }
}
