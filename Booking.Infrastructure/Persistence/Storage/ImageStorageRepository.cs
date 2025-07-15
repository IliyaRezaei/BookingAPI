using Booking.Domain.Abstractions.Storage;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Infrastructure.Persistence.Storage
{
    internal class ImageStorageRepository : IImageStorageRepository
    {
        public void StoreImage(IFormFile image, string path)
        {
            string folderPath = Path.GetDirectoryName(path);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                image.CopyTo(stream);
            }
        }

        public void DeleteImage(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}
