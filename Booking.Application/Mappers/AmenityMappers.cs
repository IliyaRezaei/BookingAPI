using Booking.Domain.Contracts.Amenity;
using Booking.Domain.Entities;

namespace Booking.Application.Mappers
{
    internal static class AmenityMappers
    {
        public static Amenity ToEntity(this CreateAmenityRequest request)
        {
            return new Amenity
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
            };
        }

        public static AmenityResponse ToResponse(this Amenity amenity)
        {
            return new AmenityResponse
            {
                Id = amenity.Id,
                Name = amenity.Name,
                ImageUrl = amenity.ImageUrl,
            };
        }

        public static IEnumerable<AmenityResponse> ToResponse(this IEnumerable<Amenity> amenities)
        {
            return amenities.Select(amenity => amenity.ToResponse()).ToList();
        }

        public static Amenity ToEntity(this UpdateAmenityRequest request, Guid amenityId)
        {
            return new Amenity
            {
                Id = amenityId,
                Name = request.Name,
            };
        }
    }
}
