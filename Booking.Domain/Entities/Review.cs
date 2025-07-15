using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Domain.Entities
{
    public class Review
    {
        public Guid Id { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; }
        public Property Property { get; set; }
        public ApplicationUser Client { get; set; }
    }
}
