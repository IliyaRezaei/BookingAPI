using Booking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Infrastructure.Data.Configurations
{
    internal class AmenityConfiguration : IEntityTypeConfiguration<Amenity>
    {
        public void Configure(EntityTypeBuilder<Amenity> builder)
        {
            builder.HasIndex(amenity => amenity.Name).IsUnique();
            builder.Property(amenity => amenity.Name).IsRequired().HasMaxLength(20);
            builder.Property(amenity => amenity.ImageUrl).IsRequired(false).HasMaxLength(int.MaxValue);
        }
    }
}
