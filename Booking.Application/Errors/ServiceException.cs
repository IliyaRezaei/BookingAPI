using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Domain.Errors
{
    public class ServiceException(int statusCode, string errorMessage) : Exception(errorMessage)
    {
        public int StatusCode { get; init; } = statusCode;
        public string ErrorMessage { get; set; } = errorMessage;
    }
}
