using Booking.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Domain.Entities
{
    public class City : IImageEntity
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required Country Country { get; set; }
        public string? ImageUrl { get; set; }
    }
}
