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
    internal class UserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            // Configure Many-to-Many Relationship: ApplicationUser-ApplicationRole
            builder
                .HasMany(u => u.Roles)
                .WithMany(r => r.Users)
                .UsingEntity("UserRoles");

            builder.HasIndex(user => user.Username).IsUnique();
            builder.HasIndex(user => user.Email).IsUnique();
            builder.Property(user => user.IsHost).HasDefaultValue(false);
            builder.Property(user => user.Username).HasMaxLength(50);
            builder.Property(user => user.Email).HasMaxLength(255);
            builder.Property(user => user.ImageUrl).HasMaxLength(int.MaxValue);
            builder.Property(user => user.RefreshToken).HasMaxLength(int.MaxValue);
            //builder.Property(user => user.RefreshTokenExpiryDate).HasDefaultValue(DateTime.UtcNow.AddDays(7));
        }
    }
}
