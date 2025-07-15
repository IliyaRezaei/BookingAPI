using Booking.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Domain.Entities
{
    public class Image : IImageEntity
    {
        public Guid Id { get; set; }
        public string? ImageUrl { get; set; }
        public ICollection<Property> Properties { get; set; }
    }
}
