using Booking.Domain.Entities;
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
    internal class PropertyConfiguration : IEntityTypeConfiguration<Property>
    {
        public void Configure(EntityTypeBuilder<Property> builder)
        {
            // Configure Property-Host Relationship
            builder
                .HasOne(p => p.Host)
                .WithMany(u => u.Properties)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deleting Host if they have Properties

            // Configure Many-to-Many Relationship: Property-Amenity
            builder
                .HasMany(p => p.Amenities)
                .WithMany(a => a.Properties);

            // Configure Many-to-Many Relationship: Property-Image
            builder
                .HasMany(p => p.Images)
                .WithMany(i => i.Properties);


            builder.OwnsOne(p => p.Address);
            builder.Property(property => property.Title).IsRequired().HasMaxLength(50);
            builder.Property(property => property.Description).IsRequired().HasMaxLength(250);

            builder.Property(property => property.PricePerNight).IsRequired();
            builder.Property(property => property.Guests).IsRequired();
            builder.Property(property => property.BedRooms).IsRequired();
            builder.Property(property => property.Beds).IsRequired();
            builder.Property(property => property.Bathrooms).IsRequired();
        }
    }
}
