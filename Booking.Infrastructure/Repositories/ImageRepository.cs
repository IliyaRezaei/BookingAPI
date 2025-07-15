using Booking.Domain.Abstractions.Repositories;
using Booking.Domain.Entities;
using Booking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Infrastructure.Repositories
{
    internal class ImageRepository : IImageRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public ImageRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task Create(Image image)
        {
            await _dbContext.Images.AddAsync(image);
        }

        public void Delete(Image image)
        {
            _dbContext.Images.Remove(image);
        }

        public async Task<IEnumerable<Image>> GetAll()
        {
            return await _dbContext.Images.ToListAsync();
        }

        public async Task<Image?> GetById(Guid id)
        {
            return await _dbContext.Images.FindAsync(id);
        }

        public void Update(Image image)
        {
            _dbContext.Images.Update(image);
        }
    }
}
