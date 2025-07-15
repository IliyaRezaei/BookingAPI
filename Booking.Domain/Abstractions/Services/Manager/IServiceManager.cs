using Booking.Domain.Abstractions.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Domain.Abstractions.Services.Manager
{
    public interface IServiceManager
    {
        IAmenityService Amenities { get; }
        ICountryService Countries { get; }
        ICityService Cities { get; }
        IPropertyService Properties { get; }
        IReviewService Reviews { get; }
        IBookingService Bookings { get; }
        IImageStorageService ImageStorage { get; }
        IRoleService Roles { get; }
        IUserService Users { get; }
    }
}
