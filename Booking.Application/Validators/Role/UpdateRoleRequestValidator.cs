using Booking.Domain.Abstractions.Repositories.Manager;
using Booking.Domain.Contracts.Role;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.Validators.Role
{
    internal class UpdateRoleRequestValidator : AbstractValidator<UpdateRoleRequest>
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly Guid _roleId;
        public UpdateRoleRequestValidator(IRepositoryManager repositoryManager, Guid roleId)
        {
            _repositoryManager = repositoryManager;
            _roleId = roleId;

            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(50)
                .MustAsync(async (name, cancellation) => await IsUniqueName(name))
                .WithMessage("Name must be unique"); ;
        }


        private async Task<bool> IsUniqueName(string name)
        {
            var role = await _repositoryManager.Roles.GetByName(name);
            if (role == null || role.Id == _roleId)
            {
                return true;
            }
            return false;
        }
    }
}
