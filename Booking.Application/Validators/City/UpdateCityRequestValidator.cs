using Booking.Domain.Abstractions.Repositories.Manager;
using Booking.Domain.Contracts.City;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.Validators.City
{
    internal class UpdateCityRequestValidator : AbstractValidator<UpdateCityRequest>
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly Guid _cityId;
        public UpdateCityRequestValidator(IRepositoryManager repositoryManager, Guid cityId)
        {
            _repositoryManager = repositoryManager;
            _cityId = cityId;

            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(20)
                .MustAsync(async (name, cancellation) => await IsUniqueName(name))
                .WithMessage("Name must be unique");

            RuleFor(x => x.CountryId)
                .MustAsync(async (countryId, cancellation) => await CountryExistsById(countryId))
                .WithMessage(countryId => $"Country with Id {countryId} does not exist");
        }

        private async Task<bool> IsUniqueName(string name)
        {
            var city = await _repositoryManager.Cities.GetByName(name);
            if (city == null || city.Id == _cityId)
            {
                return true;
            }
            return false;
        }

        private async Task<bool> CountryExistsById(Guid countryId)
        {
            var country = await _repositoryManager.Countries.GetById(countryId);
            return country != null;
        }
    }
}
