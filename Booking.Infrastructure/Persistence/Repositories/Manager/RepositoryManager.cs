using Booking.Domain.Abstractions.Repositories;
using Booking.Domain.Abstractions.Repositories.Manager;
using Booking.Domain.Abstractions.Storage;
using Booking.Infrastructure.Data;
using Booking.Infrastructure.Persistence.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Infrastructure.Persistence.Repositories.Manager
{
    internal class RepositoryManager : IRepositoryManager
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly Lazy<IPropertyRepository> _propertyRepository;
        private readonly Lazy<ICountryRepository> _countryRepository;
        private readonly Lazy<ICityRepository> _cityRepository;
        private readonly Lazy<IAmenityRepository> _amenityRepository;
        private readonly Lazy<IBookingRepository> _bookingRepository;
        private readonly Lazy<IReviewRepository> _reviewRepository;
        private readonly Lazy<IRoleRepository> _roleRepository;
        private readonly Lazy<IUserRepository> _userRepository;
        private readonly Lazy<IImageRepository> _imageRepository;
        private readonly Lazy<IImageStorageRepository> _imageStorageRepository;
        public RepositoryManager(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _propertyRepository = new Lazy<IPropertyRepository>(() => new PropertyRepository(dbContext));
            _amenityRepository = new Lazy<IAmenityRepository>(() => new AmenityRepository(dbContext));
            _bookingRepository = new Lazy<IBookingRepository>(() => new BookingRepository(dbContext));
            _reviewRepository = new Lazy<IReviewRepository>(() => new ReviewRepository(dbContext));
            _countryRepository = new Lazy<ICountryRepository>(() => new CountryRepository(dbContext));
            _cityRepository = new Lazy<ICityRepository>(() => new CityRepository(dbContext));
            _roleRepository = new Lazy<IRoleRepository>(() => new RoleRepository(dbContext));
            _userRepository = new Lazy<IUserRepository>(() => new UserRepository(dbContext));
            _imageRepository = new Lazy<IImageRepository>(() => new ImageRepository(dbContext));
            _imageStorageRepository = new Lazy<IImageStorageRepository>(() => new ImageStorageRepository());
        }

        public ICityRepository Cities => _cityRepository.Value;

        public ICountryRepository Countries => _countryRepository.Value;

        public IPropertyRepository Properties => _propertyRepository.Value;

        public IAmenityRepository Amenities => _amenityRepository.Value;

        public IBookingRepository Bookings => _bookingRepository.Value;

        public IReviewRepository Reviews => _reviewRepository.Value;

        public IRoleRepository Roles => _roleRepository.Value;

        public IUserRepository Users => _userRepository.Value;
        public IImageRepository Images => _imageRepository.Value;

        public IImageStorageRepository ImagesStorage => _imageStorageRepository.Value;


        public async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
