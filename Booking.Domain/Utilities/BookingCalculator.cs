using Booking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Domain.Utilities
{
    public static class BookingCalculator
    {
        public static int CalculateTotalCost(DateOnly CheckInDate, DateOnly CheckOutDate, int pricePerNight)
        {
            return ((CheckOutDate.ToDateTime(TimeOnly.MinValue) - CheckInDate.ToDateTime(TimeOnly.MinValue)).Days + 1) * pricePerNight;
        }
    }
}
