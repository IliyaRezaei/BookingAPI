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
    internal class CityConfiguration : IEntityTypeConfiguration<City>
    {
        public void Configure(EntityTypeBuilder<City> builder)
        {
            builder.HasIndex(city => city.Name).IsUnique();
            builder.Property(city => city.Name).IsRequired().HasMaxLength(20);
            builder.Property(city => city.NormalizedName).IsRequired().HasMaxLength(20);
            builder.Property(city => city.ImageUrl).IsRequired(false).HasMaxLength(int.MaxValue);
        }
    }
}
