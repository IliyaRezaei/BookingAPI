using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Domain.Errors
{
    public class NotFoundException(string errorMessage) 
        : ServiceException(StatusCodes.Status404NotFound, errorMessage)
    {
    }
}
