using Booking.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Domain.Entities
{
    public class Booking
    {
        public Guid Id { get; set; }
        public Property Property { get; set; }
        public ApplicationUser Client { get; set; }
        public DateOnly CheckIn { get; set; }
        public DateOnly CheckOut { get; set; }
        public int TotalCost { get; set; }
        public BookingStatus Status { get; set; }
    }
}
