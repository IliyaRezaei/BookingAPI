using Booking.Domain.Contracts.Amenity;
using Booking.Domain.Contracts.City;
using Booking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.Mappers
{
    internal static class CityMappers
    {
        public static City ToEntity(this CreateCityRequest request, Country country)
        {
            return new City
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Country = country,
            };
        }

        public static CityResponse ToResponse(this City city)
        {
            return new CityResponse
            {
                Id = city.Id,
                Name = city.Name,
                ImageUrl = city.ImageUrl,
            };
        }

        public static IEnumerable<CityResponse> ToResponse(this IEnumerable<City> cities)
        {
            return cities.Select(city => city.ToResponse()).ToList();
        }

        public static City ToEntity(this UpdateCityRequest request, Guid cityId, Country country)
        {
            return new City
            {
                Id = cityId,
                Name = request.Name,
                Country = country,
            };
        }
    }
}
