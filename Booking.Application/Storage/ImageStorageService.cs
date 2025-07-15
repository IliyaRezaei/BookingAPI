using Booking.Domain.Abstractions.Repositories.Manager;
using Booking.Domain.Abstractions.Services;
using Booking.Domain.Abstractions.Storage;
using Booking.Domain.Entities;
using Booking.Domain.Enums;
using Booking.Domain.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Booking.Application.Storage
{
    internal class ImageStorageService : IImageStorageService
    {
        private readonly IRepositoryManager _repositoryManager;
        public ImageStorageService(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }
        public async Task<string> StoreImageAsync(IFormFile image, EntityTypeImage type, Guid id, string directoryPath)
        {
            IImageEntity result = null;
            switch (type)
            {
                case EntityTypeImage.Amenity:
                    result = await _repositoryManager.Amenities.GetById(id);
                    break;
                case EntityTypeImage.Country:
                    result = await _repositoryManager.Countries.GetById(id);
                    break;
                case EntityTypeImage.City:
                    result = await _repositoryManager.Cities.GetById(id);
                    break;
                case EntityTypeImage.User:
                    break;
                    default:
                    break;
            }
            if (result == null)
            {
                throw new Exception("bad reuqest");
            }
            string extension = Path.GetExtension(image.FileName);
            string fileName = id + extension;
            string path = Path.Combine("images", type.ToString());
            string fullPath = Path.Combine(directoryPath, "wwwroot",path, fileName);
            _repositoryManager.ImagesStorage.StoreImage(image, fullPath);
            string imageUrl = Path.Combine("https://localhost:7145/", path, fileName);
            result.ImageUrl = imageUrl;
            await _repositoryManager.SaveAsync();
            return imageUrl;
        }

        public async Task<ICollection<string>> StoreImagesAsync(IFormFileCollection images, EntityTypeImage type, Guid id, string directoryPath)
        {
            var property = await _repositoryManager.Properties.GetById(id);
            if (property == null)
            {
                throw new Exception("Property with id:"+id+ " not found");
            }
            if (property.Images == null)
            {
                property.Images = new List<Domain.Entities.Image>();
            }
            ICollection<string> imageUrls = new List<string>();
            for (int i = 0; i < images.Count; i++)
            {
                string extension = Path.GetExtension(images[i].FileName);
                string fileName = i.ToString() + extension;
                string path = Path.Combine("images", type.ToString());
                string fullPath = Path.Combine(directoryPath, "wwwroot", path, id.ToString(), fileName);

                _repositoryManager.ImagesStorage.StoreImage(images[i], fullPath);

                string imageUrl = Path.Combine("https://localhost:7145/", path, id.ToString(), fileName);
                imageUrls.Add(imageUrl);
                var image = new Domain.Entities.Image
                {
                    Id = Guid.NewGuid(),
                    ImageUrl = fullPath,
                };
                await _repositoryManager.Images.Create(image);
                property.Images.Add(image);
            }
            await _repositoryManager.SaveAsync();
            return imageUrls;
        }
    }
}
