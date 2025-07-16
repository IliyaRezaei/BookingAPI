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
    internal class CreateCityRequestValidator : AbstractValidator<CreateCityRequest>
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly Guid _countryId;
        public CreateCityRequestValidator(IRepositoryManager repositoryManager, Guid countryId)
        {
            _repositoryManager = repositoryManager;
            _countryId = countryId;

            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(20)
                .MustAsync(async (name, cancellation) => await IsUniqueName(name))
                .WithMessage("Name must be unique");

            RuleFor(x => x)
                .MustAsync(async (request, cancellation) => await CountryExistsById())
                .WithMessage($"Country with Id {countryId} does not exist");
        }

        private async Task<bool> IsUniqueName(string name)
        {
            var city = await _repositoryManager.Cities.GetByName(name);
            return city == null;
        }

        private async Task<bool> CountryExistsById()
        {
            var country = await _repositoryManager.Countries.GetById(_countryId);
            return country != null;
        }
    }
}
