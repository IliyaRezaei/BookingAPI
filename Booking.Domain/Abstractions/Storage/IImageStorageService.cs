using Booking.Domain.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Domain.Abstractions.Storage
{
    public interface IImageStorageService
    {
        public Task<string> StoreImageAsync(IFormFile image, EntityTypeImage type, Guid id, string directoryPath);
        public Task<ICollection<string>> StoreImagesAsync(IFormFileCollection images, EntityTypeImage type, Guid id, string directoryPath);
    }
}
