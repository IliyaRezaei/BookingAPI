using Booking.Domain.Contracts.City;
using Booking.Domain.Contracts.Country;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Domain.Abstractions.Services
{
    public interface ICountryService
    {
        public Task<IEnumerable<CountryResponse>> GetAll();
        public Task<CountryResponse> GetById(Guid countryId);
        public Task<CountryResponse> Create(CreateCountryRequest country);
        public Task Delete(Guid countryId);
        public Task Update(UpdateCountryRequest country, Guid countryId);
    }
}
