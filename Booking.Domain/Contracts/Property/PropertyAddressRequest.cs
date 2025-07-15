using Booking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Domain.Contracts.Property
{
    public class PropertyAddressRequest
    {
        public Guid CityId { get; set; }
        public string LocationDescription { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
