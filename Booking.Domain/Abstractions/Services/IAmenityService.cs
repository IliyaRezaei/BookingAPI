using Booking.Domain.Contracts.Amenity;
using Booking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Domain.Abstractions.Services
{
    public interface IAmenityService
    {
        public Task<IEnumerable<AmenityResponse>> GetAll();
        public Task<AmenityResponse> GetById(Guid amenityId);
        public Task<AmenityResponse> Create(CreateAmenityRequest amenity);
        public Task Delete(Guid amenityId);
        public Task Update(UpdateAmenityRequest amenity, Guid amenityId);
    }
}
