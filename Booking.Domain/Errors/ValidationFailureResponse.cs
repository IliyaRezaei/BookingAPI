using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Domain.Errors
{
    public class ValidationFailureResponse
    {
        public IEnumerable<ValidationResponse> Errors { get; set; }
    }

    public class ValidationResponse
    {
        public required string PropertyName { get; set; }
        public required string ErrorMessage { get; set; }
    }
}
