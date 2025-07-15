using Booking.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Domain.Contracts.Booking
{
    public class CreateBookingRequest
    {
        public DateOnly CheckIn { get; set; }
        public DateOnly CheckOut { get; set; }
    }
}
