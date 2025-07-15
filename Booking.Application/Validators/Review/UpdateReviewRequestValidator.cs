using Booking.Domain.Contracts.Review;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.Validators.Review
{
    internal class UpdateReviewRequestValidator : AbstractValidator<UpdateReviewRequest>
    {
        public UpdateReviewRequestValidator()
        {
            RuleFor(x => x.Comment)
                .NotEmpty()
                .Length(10, 250);

            RuleFor(x => x.Rating)
                .InclusiveBetween(1, 5);
        }
    }
}
