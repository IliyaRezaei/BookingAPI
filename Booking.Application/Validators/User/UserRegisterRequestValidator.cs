using Booking.Domain.Contracts.User;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.Validators.User
{
    internal class UserRegisterRequestValidator : AbstractValidator<UserRegisterRequest>
    {
        public UserRegisterRequestValidator()
        {
            RuleFor(u => u.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");

            RuleFor(u => u.Username)
                .NotEmpty().WithMessage("Username is required")
                .Length(3, 20).WithMessage("Username must be 3 to 20 characters");

            RuleFor(u => u.Password)
                .NotEmpty().WithMessage("Password is required")
                .Length(8, 30).WithMessage("Password must be 8 to 30 characters");

            RuleFor(u => u.ConfirmPassword)
                .Equal(u => u.Password).WithMessage("Passwords do not match");
        }
    }
}
