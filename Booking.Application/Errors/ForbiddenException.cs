using Booking.Domain.Errors;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.Errors
{
    public class ForbiddenException(string errorMessage) 
        : ServiceException(StatusCodes.Status403Forbidden, errorMessage)
    {
    }
}
