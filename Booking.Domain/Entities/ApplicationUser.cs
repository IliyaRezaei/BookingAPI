using Booking.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Domain.Entities
{
    public class ApplicationUser : IImageEntity
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string NormalizedEmail { get; set; }
        public string Username { get; set; }
        public string NormalizedUsername { get; set; }
        public string HashedPassword { get; set; }
        public bool IsHost { get; set; }
        public string? ImageUrl { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryDate { get; set; }
        public ICollection<ApplicationRole> Roles { get; set; } = new List<ApplicationRole>();
        public ICollection<Property> Properties { get; set; } = new List<Property>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
