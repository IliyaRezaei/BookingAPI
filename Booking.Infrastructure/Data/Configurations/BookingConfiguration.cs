using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Infrastructure.Data.Configurations
{
    internal class BookingConfiguration : IEntityTypeConfiguration<Domain.Entities.Booking>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.Booking> builder)
        {
            // Configure Property-Bookings Relationship
            builder
                .HasOne(b => b.Property)
                .WithMany(p => p.Bookings)
                .OnDelete(DeleteBehavior.Cascade); // If a Property is deleted, its Bookings are deleted

            // Configure Client-Bookings Relationship
            builder
                .HasOne(b => b.Client)
                .WithMany(u => u.Bookings)
                .OnDelete(DeleteBehavior.Cascade); // If a Client is deleted, their Bookings are deleted


            builder
                .Property(b => b.Status)
                .HasConversion<string>(); // Store Enum as String


            builder.Property(booking => booking.CheckIn).IsRequired();
            builder.Property(booking => booking.CheckOut).IsRequired();
            builder.Property(booking => booking.TotalCost).IsRequired();
            builder.Property(booking => booking.Status).IsRequired().HasConversion<string>();
        }
    }
}
