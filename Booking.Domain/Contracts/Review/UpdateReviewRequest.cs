using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Domain.Contracts.Review
{
    public class UpdateReviewRequest
    {
        public string Comment { get; set; }
        public int Rating { get; set; }
    }
}
