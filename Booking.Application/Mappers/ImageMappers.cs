using Booking.Domain.Contracts.Image;
using Booking.Domain.Entities;

namespace Booking.Application.Mappers
{
    internal static class ImageMappers
    {
        public static Image ToEntity(this CreateImageRequest request)
        {
            return new Image
            {

            };
        }
        public static ImageResponse ToResponse(this Image image)
        {
            return new ImageResponse
            {

            };
        }
        public static IEnumerable<ImageResponse> ToResponse(this IEnumerable<Image> images)
        {
            return images.Select(i => i.ToResponse());
        }
    }
}
