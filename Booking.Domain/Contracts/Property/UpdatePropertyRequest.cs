using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Domain.Contracts.Property
{
    public class UpdatePropertyRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int PricePerNight { get; set; }


        public int BedRooms { get; set; }
        public int Beds { get; set; }
        public int Guests { get; set; }
        public int Bathrooms { get; set; }
    }
}
