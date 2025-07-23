using Booking.Domain.Contracts.Property;
using Booking.Domain.Entities;

namespace Booking.Application.Mappers
{
    internal static class PropertyMappers
    {
        public static Property ToEntity(this CreatePropertyRequest request, ApplicationUser host)
        {
            return new Property
            {
                Title = request.Title,
                Description = request.Description,
                PricePerNight = request.PricePerNight,
                BedRooms = request.BedRooms,
                Beds = request.Beds,
                Guests = request.Guests,
                Bathrooms = request.Bathrooms,
                Host = host
            };
        }
        public static Property ToEntity(this UpdatePropertyRequest request, Guid propertyId)
        {
            return new Property
            {
                Title = request.Title,
                Description = request.Description,
                PricePerNight = request.PricePerNight,
                BedRooms = request.BedRooms,
                Beds = request.Beds,
                Guests = request.Guests,
                Bathrooms = request.Bathrooms,
            };
        }

        public static PropertyResponse ToResponse(this Property property)
        {
            return new PropertyResponse
            {
                Id = property.Id,
                Title = property.Title,
                Description = property.Description,
                PricePerNight = property.PricePerNight,
                BedRooms = property.BedRooms,
                Beds = property.Beds,
                Bathrooms = property.Bathrooms,
                Guests = property.Guests,
                Address = property.Address,
            };
        }
        public static IEnumerable<PropertyResponse> ToResponse(this IEnumerable<Property> properties)
        {
            return properties.Select(p => p.ToResponse());
        }
    }
}
