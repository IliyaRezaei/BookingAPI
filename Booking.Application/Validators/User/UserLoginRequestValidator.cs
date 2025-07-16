using Booking.Application.Utilities;
using Booking.Domain.Abstractions.Repositories.Manager;
using Booking.Domain.Contracts.User;
using Booking.Domain.Entities;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.Validators.User
{
    internal class UserLoginRequestValidator : AbstractValidator<UserLoginRequest>
    {
        private readonly IRepositoryManager _repositoryManager;
        public UserLoginRequestValidator(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;

            RuleFor(u => u.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");

            RuleFor(u => u.Password)
               .NotEmpty().WithMessage("Password is required")
               .Length(8, 30).WithMessage("Password must be 8 to 30 characters");

            RuleFor(x => x)
                .MustAsync(async (request, cancellation) => await ValidateUser(request))
                .WithMessage("Credentials that you provided are invalid");
        }

        private async Task<bool> ValidateUser(UserLoginRequest request)
        {
            var user = await _repositoryManager.Users.GetByEmail(request.Email);
            if (user != null && user.HashedPassword == request.Password.HashPassword())
            {
                return true;
            }
            return false;
        }
    }
}
