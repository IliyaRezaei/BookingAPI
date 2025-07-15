using Booking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Domain.ValueObjects
{
    public class Address
    {
        public Country? Country { get; set; }
        public City? City { get; set; }
        public string? LocationDescription { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}
