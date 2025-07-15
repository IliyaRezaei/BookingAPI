using Booking.Domain.Abstractions.Repositories.Manager;
using Booking.Domain.Contracts.Country;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.Validators.Country
{
    internal class UpdateCountryRequestValidator : AbstractValidator<UpdateCountryRequest>
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly Guid _countryId;
        public UpdateCountryRequestValidator(IRepositoryManager repositoryManager, Guid countryId)
        {
            _repositoryManager = repositoryManager;
            _countryId = countryId;

            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(50)
                .MustAsync(async (name, cancellation) => await IsUniqueName(name))
                .WithMessage("Name must be unique");
        }

        private async Task<bool> IsUniqueName(string name)
        {
            var country = await _repositoryManager.Countries.GetByName(name);
            if (country == null)
            {
                return true;
            }
            if (country.Id == _countryId)
            {
                return true;
            }
            return false;
        }
    }
}
