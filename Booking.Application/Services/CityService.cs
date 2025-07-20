using Booking.Application.Mappers;
using Booking.Application.Validators.City;
using Booking.Domain.Abstractions.Repositories.Manager;
using Booking.Domain.Abstractions.Services;
using Booking.Domain.Contracts.Amenity;
using Booking.Domain.Contracts.City;
using Booking.Domain.Errors;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.Services
{
    public class CityService : ICityService
    {
        private readonly IRepositoryManager _repositoryManager;
        public CityService(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }
        public async Task<CityResponse> Create(CreateCityRequest city, Guid countryId)
        {
            var validator = new CreateCityRequestValidator(_repositoryManager);
            await validator.ValidateAndThrowAsync(city);

            var country = await _repositoryManager.Countries.GetById(countryId);
            if (country == null)
            {
                throw new NotFoundException($"Country with id {countryId} not found");
            }
            var entity = city.ToEntity(country);
            await _repositoryManager.Cities.Create(entity);
            await _repositoryManager.SaveAsync();
            return entity.ToResponse();
        }

        public async Task Delete(Guid cityId)
        {
            var entity = await _repositoryManager.Cities.GetById(cityId);
            if (entity == null)
            {
                throw new NotFoundException("City with id:" + cityId + " not found");
            }
            _repositoryManager.Cities.Delete(entity);
            await _repositoryManager.SaveAsync();
        }

        public async Task<IEnumerable<CityResponse>> GetAll()
        {
            var cities = await _repositoryManager.Cities.GetAll();
            return cities.ToResponse();
        }

        public async Task<IEnumerable<CityResponse>> GetAllByCountryId(Guid countryId)
        {
            var cities = await _repositoryManager.Cities.GetAllByCountryId(countryId);
            return cities.ToResponse();
        }

        public async Task<CityResponse> GetById(Guid cityId)
        {
            var city = await _repositoryManager.Cities.GetById(cityId);
            if (city == null)
            {
                throw new NotFoundException("City with id:" + cityId + " not found");
            }
            return city.ToResponse();
        }

        public async Task Update(UpdateCityRequest request, Guid cityId)
        {
            var validator = new UpdateCityRequestValidator(_repositoryManager, cityId);
            await validator.ValidateAndThrowAsync(request);

            var city = await _repositoryManager.Cities.GetById(cityId);
            if (city == null)
            {
                throw new NotFoundException($"City with id {cityId} not found");
            }
            var newCountry = await _repositoryManager.Countries.GetById(request.CountryId);
            if (newCountry == null)
            {
                throw new NotFoundException($"Country with id {request.CountryId} not found");
            }
            var entity = request.ToEntity(cityId, newCountry);
            _repositoryManager.Cities.Update(entity);
            await _repositoryManager.SaveAsync();
        }
    }
}
