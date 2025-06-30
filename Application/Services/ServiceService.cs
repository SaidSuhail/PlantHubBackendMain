using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interface;
using AutoMapper;
using Domain.Model;

namespace Application.Services
{
   public class ServiceService:IServiceService
    {
        private readonly IServiceRepository _repository;
        private readonly IMapper _mapper;

        public ServiceService(IServiceRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<IEnumerable<ServiceDto>> GetAllServicesAsync()
        {
            var service = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<ServiceDto>>(service);
        }
        public async Task<ServiceDto>GetServiceByIdAsync(int id)
        {
            var service = await _repository.GetByIdAsync(id);
            return _mapper.Map<ServiceDto>(service);
        }
        public async Task<ServiceDto> CreateServiceAsync(CreateServiceDto dto)
        {
            var service = _mapper.Map<Service>(dto);
            var created = await _repository.AddAsync(service);
            return _mapper.Map<ServiceDto>(created);
        }
        public async Task<ServiceDto>UpdateServiceAsync(int id,UpdateServiceDto dto)
        {
            var service = await _repository.GetByIdAsync(id);
            if (service == null) throw new Exception("service not found");

            // Only update the fields that are provided
            if (!string.IsNullOrEmpty(dto.Name)) service.Name = dto.Name;
            if (!string.IsNullOrEmpty(dto.Description)) service.Description = dto.Description;
            if (dto.Price.HasValue) service.Price = dto.Price.Value;
            if (dto.EstimatedDuration.HasValue) service.EstimatedDuration = dto.EstimatedDuration.Value;

            //_mapper.Map(dto, service);
            var updated = await _repository.UpdateAsync(service);
            return _mapper.Map<ServiceDto>(updated);
        }
        public async Task<bool>DeleteServiceAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }
    }
}
