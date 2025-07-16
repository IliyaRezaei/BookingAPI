using Booking.Domain.Contracts.City;
using Booking.Domain.Contracts.Country;
using Booking.Domain.Entities;

namespace Booking.Application.Mappers
{
    internal static class CountryMappers
    {
        public static Country ToEntity(this CreateCountryRequest request)
        {
            return new Country
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                NormalizedName = request.Name.ToUpper()
            };
        }

        public static CountryResponse ToResponse(this Country country)
        {
            return new CountryResponse
            {
                Id = country.Id,
                Name = country.Name,
                ImageUrl = country.ImageUrl,
                Cities = country.Cities?.ToResponse() ?? new List<CityResponse>(),
            };
        }

        public static IEnumerable<CountryResponse> ToResponse(this IEnumerable<Country> countries)
        {
            return countries.Select(country => country.ToResponse()).ToList();
        }

        public static Country ToEntity(this UpdateCountryRequest request, Guid countryId)
        {
            return new Country
            {
                Id = countryId,
                Name = request.Name,
                NormalizedName = request.Name.ToUpper()
            };
        }
    }
}
