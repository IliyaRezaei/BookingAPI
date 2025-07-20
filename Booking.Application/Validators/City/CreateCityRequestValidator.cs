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
        public CreateCityRequestValidator(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;

            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(20)
                .MustAsync(async (name, cancellation) => await IsUniqueName(name))
                .WithMessage("Name must be unique");
        }

        private async Task<bool> IsUniqueName(string name)
        {
            var city = await _repositoryManager.Cities.GetByName(name);
            return city == null;
        }
    }
}
