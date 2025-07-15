using Booking.Application.Storage;
using Booking.Domain.Abstractions.Repositories.Manager;
using Booking.Domain.Abstractions.Services;
using Booking.Domain.Abstractions.Services.Manager;
using Booking.Domain.Abstractions.Storage;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.Services.Manager
{
    internal class ServiceManager : IServiceManager
    {
        private readonly Lazy<IAmenityService> _amenityService;
        private readonly Lazy<IPropertyService> _propertyService;
        private readonly Lazy<IBookingService> _bookingService;
        private readonly Lazy<IReviewService> _reviewService;
        private readonly Lazy<ICountryService> _countryService;
        private readonly Lazy<ICityService> _cityService;
        private readonly Lazy<IImageStorageService> _imageStorageService;
        private readonly Lazy<IRoleService> _roleService;
        private readonly Lazy<IUserService> _userService;
        private readonly IConfiguration _configuration;
        public ServiceManager(IRepositoryManager repositoryManager, IConfiguration configuration)
        {
            _configuration = configuration;
            _amenityService = new Lazy<IAmenityService>(() => new AmenityService(repositoryManager));
            _countryService = new Lazy<ICountryService>(() => new CountryService(repositoryManager));
            _cityService = new Lazy<ICityService>(() => new CityService(repositoryManager));
            _imageStorageService = new Lazy<IImageStorageService> (() => new ImageStorageService(repositoryManager));
            _propertyService = new Lazy<IPropertyService>(() => new PropertyService(repositoryManager));
            _bookingService = new Lazy<IBookingService>(() => new BookingService(repositoryManager));
            _reviewService = new Lazy<IReviewService>(() => new ReviewService(repositoryManager));
            _roleService = new Lazy<IRoleService>(() => new RoleService(repositoryManager));
            _userService = new Lazy<IUserService>(() => new UserService(repositoryManager, _configuration));
        }
        public IAmenityService Amenities => _amenityService.Value;

        public ICountryService Countries => _countryService.Value;

        public ICityService Cities => _cityService.Value;

        public IPropertyService Properties => _propertyService.Value;

        public IReviewService Reviews => _reviewService.Value;

        public IBookingService Bookings => _bookingService.Value;

        public IImageStorageService ImageStorage => _imageStorageService.Value;

        public IRoleService Roles => _roleService.Value;

        public IUserService Users => _userService.Value;
    }
}
