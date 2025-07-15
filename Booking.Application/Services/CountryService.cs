using Booking.Application.Mappers;
using Booking.Application.Validators.Country;
using Booking.Domain.Abstractions.Repositories.Manager;
using Booking.Domain.Abstractions.Services;
using Booking.Domain.Contracts.Amenity;
using Booking.Domain.Contracts.Country;
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
    public class CountryService : ICountryService
    {
        private readonly IRepositoryManager _repositoryManager;
        public CountryService(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }
        public async Task<CountryResponse> Create(CreateCountryRequest country)
        {
            var validator = new CreateCountryRequestValidator(_repositoryManager);
            await validator.ValidateAndThrowAsync(country);

            var entity = country.ToEntity();
            await _repositoryManager.Countries.Create(entity);
            await _repositoryManager.SaveAsync();
            return entity.ToResponse();
        }

        public async Task Delete(Guid countryId)
        {
            var entity = await _repositoryManager.Countries.GetById(countryId);
            if (entity == null)
            {
                throw new Exception("Country with id:" + countryId + " not found");
            }
            _repositoryManager.Countries.Delete(entity);
            await _repositoryManager.SaveAsync();
        }

        public async Task<IEnumerable<CountryResponse>> GetAll()
        {
            var countries = await _repositoryManager.Countries.GetAll();
            return countries.ToResponse();
        }

        public async Task<CountryResponse> GetById(Guid countryId)
        {
            var country = await _repositoryManager.Countries.GetById(countryId);
            var cities = await _repositoryManager.Cities.GetAllByCountryId(countryId);
            if (country == null)
            {
                throw new Exception("Country with id:" + countryId + " not found");
            }
            country.Cities = cities.ToList();
            return country.ToResponse();
        }

        public async Task Update(UpdateCountryRequest request, Guid countryId)
        {
            var validator = new UpdateCountryRequestValidator(_repositoryManager, countryId);
            await validator.ValidateAndThrowAsync(request);

            var country = await _repositoryManager.Countries.GetById(countryId);
            if (country == null)
            {
                throw new NotFoundException($"Country with id {countryId} not found");
            }
            var entity = request.ToEntity(countryId);
            _repositoryManager.Countries.Update(entity);
            await _repositoryManager.SaveAsync();
        }
    }
}
