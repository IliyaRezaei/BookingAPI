using Booking.Domain.Contracts.City;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Domain.Contracts.Country
{
    public class CountryResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public IEnumerable<CityResponse>? Cities { get; set; }
    }
}
