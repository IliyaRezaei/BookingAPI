using Booking.Application.Errors;
using Booking.Application.Mappers;
using Booking.Application.Validators.Property;
using Booking.Domain.Abstractions.Repositories.Manager;
using Booking.Domain.Abstractions.Services;
using Booking.Domain.Abstractions.Services.Manager;
using Booking.Domain.Contracts.Property;
using Booking.Domain.Entities;
using Booking.Domain.Errors;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.Services
{
    internal class PropertyService : IPropertyService
    {
        private readonly IRepositoryManager _repositoryManager;
        public PropertyService(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }

        public async Task AddAddress(PropertyAddressRequest request, Guid propertyId, string username)
        {
            var user = await _repositoryManager.Users.GetByUsername(username);
            if (user == null) 
            {
                throw new UnauthorizedException("Login and try again");
            }
            var userProperties = await _repositoryManager.Properties.GetAllPropertiesOfAUser(user);
            var property = userProperties.Where(p => p.Id == propertyId).FirstOrDefault();
            if (property == null)
            {
                throw new Exception("Property with id:" + propertyId + " not found");
            }
            var city = await _repositoryManager.Cities.GetById(request.CityId);
            if (city == null)
            {
                throw new NotFoundException("City with id:" + request.CityId + " not found");
            }
            property.Address =
                new Domain.ValueObjects.Address
                {
                    City = city,
                    Country = city.Country,
                    LocationDescription = request.LocationDescription,
                    Longitude = request.Longitude,
                    Latitude = request.Latitude,
                };
            await _repositoryManager.SaveAsync();
        }

        public async Task AddAmenities(PropertyAmenitiesRequest request, Guid propertyId, string username)
        {
            var user = await _repositoryManager.Users.GetByUsername(username);
            if (user == null)
            {
                throw new UnauthorizedException("Login and try again");
            }
            var userProperties = await _repositoryManager.Properties.GetAllPropertiesOfAUser(user);
            var property = userProperties.Where(p => p.Id == propertyId).FirstOrDefault();
            if (property == null)
            {
                throw new NotFoundException("Property with id:" + propertyId + " not found");
            }
            var amenities = await _repositoryManager.Amenities.GetAllByIds(request.Amenities);
            property.Amenities = amenities.ToList();
            await _repositoryManager.SaveAsync();
        }

        public async Task<PropertyResponse> Create(CreatePropertyRequest request, string username)
        {
            var validator = new CreatePropertyRequestValidator(_repositoryManager);
            await validator.ValidateAndThrowAsync(request);

            var user = await _repositoryManager.Users.GetByUsername(username);
            if (user == null)
            {
                throw new UnauthorizedException("Login and try again");
            }
            if (!user.IsHost)
            {
                throw new ForbiddenException("Only hosts are allowed to create properties");
            }
            var entity = request.ToEntity(user);
            await _repositoryManager.Properties.Create(entity);
            await _repositoryManager.SaveAsync();
            return entity.ToResponse();
        }

        public async Task Delete(Guid propertyId, string username)
        {
            var user = await _repositoryManager.Users.GetByUsername(username);
            if (user == null)
            {
                throw new UnauthorizedException("Login and try again");
            }
            var userProperties = await _repositoryManager.Properties.GetAllPropertiesOfAUser(user);
            var property = userProperties.Where(p => p.Id == propertyId).FirstOrDefault();
            if (property == null)
            {
                throw new NotFoundException($"Property with id {propertyId} not found");
            }
            _repositoryManager.Properties.Delete(property);
            await _repositoryManager.SaveAsync();
        }

        public async Task<IEnumerable<PropertyResponse>> GetAll()
        {
            var properties = await _repositoryManager.Properties.GetAll();
            var response = properties.ToResponse();
            return response;
        }

        public async Task<PropertyResponse> GetById(Guid propertyId)
        {
            var property = await _repositoryManager.Properties.GetById(propertyId);
            if (property == null)
            {
                throw new NotFoundException($"Property with id {propertyId} not found");
            }
            return property.ToResponse();
        }

        public async Task Update(UpdatePropertyRequest request, Guid propertyId, string username)
        {
            var validator = new UpdatePropertyRequestValidator();
            await validator.ValidateAndThrowAsync(request);

            var user = await _repositoryManager.Users.GetByUsername(username);
            if (user == null)
            {
                throw new UnauthorizedException("Login and try again");
            }
            var userProperties = await _repositoryManager.Properties.GetAllPropertiesOfAUser(user);
            var property = userProperties.Where(p => p.Id == propertyId).FirstOrDefault();
            if (property == null)
            {
                throw new NotFoundException($"Property with id {propertyId} not found");
            }
            var updateProperty = request.ToEntity(propertyId);
            _repositoryManager.Properties.Update(property);
            await _repositoryManager.SaveAsync();
        }
    }
}
