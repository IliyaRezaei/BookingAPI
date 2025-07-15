using Booking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Domain.Abstractions.Repositories
{
    public interface IImageRepository
    {
        public Task<IEnumerable<Image>> GetAll();
        public Task<Image?> GetById(Guid id);
        public Task Create(Image image);
        public void Delete(Image image);
        public void Update(Image image);
    }
}
