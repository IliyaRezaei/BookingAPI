using Booking.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Domain.Entities
{
    public class Country : IImageEntity
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string NormalizedName { get; set; }
        public ICollection<City>? Cities { get; set; } = new List<City>();
        public string? ImageUrl { get; set; }
    }
}
