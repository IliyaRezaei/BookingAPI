using Booking.Application.Mappers;
using Booking.Application.Validators.Role;
using Booking.Domain.Abstractions.Repositories.Manager;
using Booking.Domain.Abstractions.Services;
using Booking.Domain.Contracts.Role;
using Booking.Domain.Errors;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRepositoryManager _repositoryManager;
        public RoleService(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }
        public async Task<RoleResponse> Create(CreateRoleRequest request)
        {
            var validator = new CreateRoleRequestValidator(_repositoryManager);
            await validator.ValidateAndThrowAsync(request);

            var role = request.ToEntity();
            await _repositoryManager.Roles.Create(role);
            await _repositoryManager.SaveAsync();
            return role.ToResponse();
        }

        public async Task Delete(Guid id)
        {
            var role = await _repositoryManager.Roles.GetById(id);
            if(role == null)
            {
                throw new NotFoundException($"Role with id {id} not found");
            }
            _repositoryManager.Roles.Delete(role);
            await _repositoryManager.SaveAsync();
        }

        public async Task<IEnumerable<RoleResponse>> GetAll()
        {
            var roles = await _repositoryManager.Roles.GetAll();
            return roles.ToResponse();
        }

        public async Task<RoleResponse> GetById(Guid id)
        {
            var role = await _repositoryManager.Roles.GetById(id);
            if (role == null)
            {
                throw new NotFoundException($"Role with id {id} not found");
            }
            return role.ToResponse();
        }

        public async Task Update(UpdateRoleRequest request, Guid id)
        {
            var validator = new UpdateRoleRequestValidator(_repositoryManager, id);
            await validator.ValidateAndThrowAsync(request);

            var role = await _repositoryManager.Roles.GetById(id);
            if (role == null)
            {
                throw new NotFoundException($"Role with id {id} not found");
            }
            var entity = request.ToEntity(id);
            _repositoryManager.Roles.Update(entity);
            await _repositoryManager.SaveAsync();
        }
    }
}
