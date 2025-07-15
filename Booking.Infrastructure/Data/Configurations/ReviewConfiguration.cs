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
    internal class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            // Configure Property-Reviews Relationship
            builder
                .HasOne(r => r.Property)
                .WithMany(p => p.Reviews)
                .OnDelete(DeleteBehavior.Cascade); // If a Property is deleted, its Reviews are deleted

            // Configure Client-Reviews Relationship
            builder
                .HasOne(r => r.Client)
                .WithMany(u => u.Reviews)
                .OnDelete(DeleteBehavior.Cascade); // If a Client is deleted, their Reviews are deleted


            builder.Property(review => review.Comment).IsRequired().HasMaxLength(250);
            builder.Property(review => review.Rating).IsRequired();
        }
    }
}
