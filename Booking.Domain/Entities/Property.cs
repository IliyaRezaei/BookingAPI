using Booking.Domain.Enums;
using Booking.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Domain.Entities
{
    public class Property
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int PricePerNight { get; set; }


        public int BedRooms { get; set; }
        public int Beds { get; set; }
        public int Guests { get; set; }
        public int Bathrooms { get; set; }

        public Address Address { get; set; }
        public ApplicationUser Host { get; set; }
        public ICollection<Image> Images { get; set; } = new List<Image>();
        public ICollection<Amenity> Amenities { get; set; } = new List<Amenity>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
