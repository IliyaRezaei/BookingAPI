using Booking.Domain.Contracts.Amenity;
using Booking.Domain.Contracts.City;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Domain.Abstractions.Services
{
    public interface ICityService
    {
        public Task<IEnumerable<CityResponse>> GetAll();
        public Task<IEnumerable<CityResponse>> GetAllByCountryId(Guid countryId);
        public Task<CityResponse> GetById(Guid cityId);
        public Task<CityResponse> Create(CreateCityRequest city, Guid countryId);
        public Task Delete(Guid cityId);
        public Task Update(UpdateCityRequest city, Guid cityId);
    }
}
