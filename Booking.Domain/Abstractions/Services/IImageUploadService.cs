using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Domain.Abstractions.Services
{
    public interface IImageUploadService
    {
        Task<string> Upload(IFormFile image, Guid id, ImageEntityType type);
    }
    public enum ImageEntityType
    {
        Amenity,
        Country,
        City
    }
}
