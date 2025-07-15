using Booking.Domain.Entities;
using Booking.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Domain.Contracts.Booking
{
    public class BookingResponse
    {
        public Guid Id { get; set; }
        public DateOnly CheckIn { get; set; }
        public DateOnly CheckOut { get; set; }
        public int TotalCost { get; set; }
        public BookingStatus Status { get; set; }
    }
}
