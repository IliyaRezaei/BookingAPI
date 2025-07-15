using Booking.Domain.Contracts.User;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.Validators.User
{
    internal class UserLoginRequestValidator : AbstractValidator<UserLoginRequest>
    {
        public UserLoginRequestValidator()
        {
            RuleFor(u => u.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");

            RuleFor(u => u.Password)
               .NotEmpty().WithMessage("Password is required")
               .Length(8, 30).WithMessage("Password must be 8 to 30 characters");
        }
    }
}
