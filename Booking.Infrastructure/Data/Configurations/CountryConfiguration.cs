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
    internal class CountryConfiguration : IEntityTypeConfiguration<Country>
    {
        public void Configure(EntityTypeBuilder<Country> builder)
        {
            builder.HasIndex(country => country.Name).IsUnique();
            builder.Property(country => country.Name).IsRequired().HasMaxLength(20);
            builder.Property(country => country.ImageUrl).IsRequired(false).HasMaxLength(int.MaxValue);
        }
    }
}
