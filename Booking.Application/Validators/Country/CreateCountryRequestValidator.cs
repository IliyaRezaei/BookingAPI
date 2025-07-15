using Booking.Domain.Abstractions.Repositories.Manager;
using Booking.Domain.Contracts.Country;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Booking.Application.Validators.Country
{
    internal class CreateCountryRequestValidator : AbstractValidator<CreateCountryRequest>
    {
        private readonly IRepositoryManager _repositoryManager;
        public CreateCountryRequestValidator(IRepositoryManager repositoryManager)
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
            var country = await _repositoryManager.Countries.GetByName(name);
            return country == null;
        }
    }
}
