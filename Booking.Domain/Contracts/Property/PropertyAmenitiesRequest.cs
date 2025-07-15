using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Domain.Contracts.Property
{
    public class PropertyAmenitiesRequest
    {
        public IEnumerable<Guid> Amenities { get; set; }
    }
}
