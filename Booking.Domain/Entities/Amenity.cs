using Booking.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Domain.Entities
{
    public class Amenity : IImageEntity
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string NormalizedName { get; set; }
        public string? ImageUrl { get; set; }
        public ICollection<Property>? Properties { get; set; }
    }
}
