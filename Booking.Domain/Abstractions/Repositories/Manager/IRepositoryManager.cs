using Booking.Domain.Abstractions.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Domain.Abstractions.Repositories.Manager
{
    public interface IRepositoryManager
    {
        IRoleRepository Roles { get; }
        IUserRepository Users { get; }
        ICityRepository Cities { get; }
        ICountryRepository Countries { get; }
        IPropertyRepository Properties { get; }
        IAmenityRepository Amenities { get; }
        IBookingRepository Bookings { get; }
        IReviewRepository Reviews { get; }
        IImageRepository Images { get; }
        IImageStorageRepository ImagesStorage { get; }
        Task SaveAsync();
    }
}
