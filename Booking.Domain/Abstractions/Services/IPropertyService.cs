using Booking.Domain.Contracts.Amenity;
using Booking.Domain.Contracts.Property;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Domain.Abstractions.Services
{
    public interface IPropertyService
    {
        public Task<IEnumerable<PropertyResponse>> GetAll();
        public Task<PropertyResponse> GetById(Guid propertyId);
        public Task<PropertyResponse> Create(CreatePropertyRequest property, string username);
        public Task Delete(Guid propertyId, string username);
        public Task Update(UpdatePropertyRequest property, Guid propertyId, string username);
        public Task AddAmenities(PropertyAmenitiesRequest request, Guid propertyId, string username);
        public Task AddAddress(PropertyAddressRequest request, Guid propertyId, string username);
    }
}
