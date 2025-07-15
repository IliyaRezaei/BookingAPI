using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Domain.Abstractions.Storage
{
    public interface IImageStorageRepository
    {
        public void StoreImage(IFormFile image, string path);
        public void DeleteImage(string path);
    }
}
