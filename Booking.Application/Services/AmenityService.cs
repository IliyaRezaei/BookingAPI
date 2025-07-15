using Booking.Application.Mappers;
using Booking.Application.Validators.Amenity;
using Booking.Domain.Abstractions.Repositories.Manager;
using Booking.Domain.Abstractions.Services;
using Booking.Domain.Contracts.Amenity;
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
    public class AmenityService : IAmenityService
    {
        private readonly IRepositoryManager _repositoryManager;
        public AmenityService(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }
        public async Task<AmenityResponse> Create(CreateAmenityRequest amenity)
        {
            var validator = new CreateAmenityRequestValidator(_repositoryManager);
            await validator.ValidateAndThrowAsync(amenity);

            var entity = amenity.ToEntity();
            await _repositoryManager.Amenities.Create(entity);
            await _repositoryManager.SaveAsync();
            return entity.ToResponse();
        }

        public async Task Delete(Guid amenityId)
        {
            var amenity = await _repositoryManager.Amenities.GetById(amenityId);
            if (amenity == null)
            {
                throw new NotFoundException($"Amenity with id {amenityId} not found");
            }
            _repositoryManager.Amenities.Delete(amenity);
            await _repositoryManager.SaveAsync();
        }

        public async Task<IEnumerable<AmenityResponse>> GetAll()
        {
            var amenities = await _repositoryManager.Amenities.GetAll();
            return amenities.ToResponse();
        }

        public async Task<AmenityResponse> GetById(Guid amenityId)
        {
            var amenity = await _repositoryManager.Amenities.GetById(amenityId);
            if (amenity == null)
            {
                throw new NotFoundException($"Amenity with id {amenityId} not found");
            }
            return amenity.ToResponse();
        }

        public async Task Update(UpdateAmenityRequest request, Guid amenityId)
        {
            var validator = new UpdateAmenityRequestValidator(_repositoryManager, amenityId);
            await validator.ValidateAndThrowAsync(request);

            var amenity = await _repositoryManager.Amenities.GetById(amenityId);
            if (amenity == null)
            {
                throw new NotFoundException($"Amenity with id {amenityId} not found");
            }
            var entity = request.ToEntity(amenityId);
            _repositoryManager.Amenities.Update(entity);
            await _repositoryManager.SaveAsync();
        }
    }
}
