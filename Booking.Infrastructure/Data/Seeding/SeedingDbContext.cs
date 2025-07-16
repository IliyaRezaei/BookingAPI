using Booking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Infrastructure.Data.Seeding
{
    public static class SeedingDbContext
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            var country = new Country
            {
                Id = Guid.NewGuid(),
                Name = "Iran",
                NormalizedName = "IRAN"
            };
            var cities = new List<City>
            {
                new City { Id = Guid.NewGuid(), Name = "Mashhad", NormalizedName = "MASHHAD", Country = country },
                new City { Id = Guid.NewGuid(), Name = "Tehran", NormalizedName = "TEHRAN", Country = country },
                new City { Id = Guid.NewGuid(), Name = "Ahvaz", NormalizedName = "AHVAZ", Country = country },
            };
            var amenities = new List<Amenity>
            {
                new Amenity { Id = Guid.NewGuid(), Name = "Cooler", NormalizedName = "COOLER" },
                new Amenity { Id = Guid.NewGuid(), Name = "Heater", NormalizedName = "HEATER" },
                new Amenity { Id = Guid.NewGuid(), Name = "Elevator", NormalizedName = "ELEVATOR" },
            };
            var roles = new List<ApplicationRole>
            {
                new ApplicationRole { Id = Guid.NewGuid(), Name = "USER", NormalizedName = "USER" },
                new ApplicationRole { Id = Guid.NewGuid(), Name = "MANAGER", NormalizedName = "MANAGER" },
                new ApplicationRole { Id = Guid.NewGuid(), Name = "ADMIN", NormalizedName = "ADMIN" },
            };
            var users = new List<ApplicationUser>
            {
                new ApplicationUser { Id = Guid.NewGuid(), Email = "user@gmail.com", 
                    NormalizedEmail = "USER@GMAIL.COM", Username = "user", NormalizedUsername = "USER",
                    HashedPassword = HashPassword("User1234@") },

                new ApplicationUser { Id = Guid.NewGuid(), Email = "manager@gmail.com", 
                    NormalizedEmail = "MANAGER@GMAIL.COM", Username = "manager", NormalizedUsername = "MANAGER",
                    HashedPassword = HashPassword("Manager1234@") },

                new ApplicationUser { Id = Guid.NewGuid(), Email = "admin@gmail.com", 
                    NormalizedEmail = "ADMIN@GMAIL.COM", Username = "admin", NormalizedUsername = "ADMIN",
                    HashedPassword = HashPassword("Admin1234@") },
            };
            var property = new Property
            {
                Id = Guid.NewGuid(),
                Title = "Title1",
                Description = "Description1",
                PricePerNight = 10,
                Host = users[0],
                BedRooms = 1,
                Beds = 1,
                Bathrooms = 1,
                Guests = 1,
                Address = new Domain.ValueObjects.Address
                {
                    Country = country,
                    City = cities[0],
                    LocationDescription = "Azadi Blvd",
                    Longitude = 12345,
                    Latitude = 12345
                }
            };

            if (!context.Countries.Any() && !context.Cities.Any())
            {
                await context.Countries.AddAsync(country);
                await context.Cities.AddRangeAsync(cities);
            }
            if (!context.Amenities.Any())
            {
                await context.Amenities.AddRangeAsync(amenities);
            }
            if (!context.Roles.Any())
            {
                await context.Roles.AddRangeAsync(roles);
            }
            if (!context.Users.Any())
            {
                await context.Users.AddRangeAsync(users);
            }
            if(!context.Properties.Any())
            {
                await context.Properties.AddAsync(property);
            }
            await context.SaveChangesAsync();
        }
        internal static void Seed(this ModelBuilder modelBuilder)
        {
            /*
            var country = new Country
            {
                Id = Guid.NewGuid(),
                Name = "Iran"
            };
            modelBuilder.Entity<Country>().HasData(country);

            var cities = new List<City>
            {
                new City { Id = Guid.NewGuid(), Name = "Mashhad", Country = country },
                new City { Id = Guid.NewGuid(), Name = "Tehran", Country = country },
                new City { Id = Guid.NewGuid(), Name = "Ahvaz", Country = country },
            };
            modelBuilder.Entity<City>().HasData(cities);

            var amenities = new List<Amenity>
            {
                new Amenity { Id = Guid.NewGuid(), Name = "Cooler" },
                new Amenity { Id = Guid.NewGuid(), Name = "Heater" },
                new Amenity { Id = Guid.NewGuid(), Name = "Elevator" },
            };
            modelBuilder.Entity<Amenity>().HasData(amenities);

            var roles = new List<ApplicationRole>
            {
                new ApplicationRole { Id = Guid.NewGuid(), Name = "USER" },
                new ApplicationRole { Id = Guid.NewGuid(), Name = "MANAGER" },
                new ApplicationRole { Id = Guid.NewGuid(), Name = "ADMIN" },
            };
            modelBuilder.Entity<ApplicationRole>().HasData(roles);

            var users = new List<ApplicationUser>
            { 
                new ApplicationUser { Id = Guid.NewGuid(), Email = "user@gmail.com", Username = "user", HashedPassword = HashPassword("User1234@") },
                new ApplicationUser { Id = Guid.NewGuid(), Email = "manager@gmail.com", Username = "manager", HashedPassword = HashPassword("Manager1234@") },
                new ApplicationUser { Id = Guid.NewGuid(), Email = "admin@gmail.com", Username = "admin", HashedPassword = HashPassword("Admin1234@") },
            };
            modelBuilder.Entity<ApplicationUser>().HasData(users);

            var property = new Property
            {
                Id = Guid.NewGuid(),
                Title = "Title1",
                Description = "Description1",
                PricePerNight = 10,
                BedRooms = 1,
                Beds = 1,
                Bathrooms = 1,
                Guests = 1,
                Address = new Domain.ValueObjects.Address
                {
                    Country = country,
                    City = cities[0],
                    LocationDescription = "Azadi Blvd",
                    Longitude = 12345,
                    Latitude = 12345
                }
            };
            modelBuilder.Entity<Property>().HasData(property);
            */
        }

        private static string HashPassword(string password )
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                var hashedPassword = Convert.ToBase64String(bytes);
                return hashedPassword;
            }
        }
    }
}
