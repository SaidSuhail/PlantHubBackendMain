using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interface;
using AutoMapper;
using Domain.Model;
using Microsoft.AspNetCore.Http;

namespace Application.Services
{
   public class PlanService:IPlanService
    {
        private readonly IPlanRepository _planRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PlanService(IPlanRepository planRepository,IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _planRepository = planRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<IEnumerable<PlanDto>> GetAllPlansAsync()
        {
            var plans = await _planRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<PlanDto>>(plans);
        }
        public async Task<PlanDto?>GetPlanByIdAsync(int id)
        {
            var plan = await _planRepository.GetByIdAsync(id);
            return _mapper.Map<PlanDto>(plan);
        }
        public async Task<PlanDto> CreatePlanAsync(PlanDto planDto)
        {
            var plan = _mapper.Map<Plan>(planDto);
            plan.BillingCycle = Domain.Enum.BillingCycle.Quarterly;
            var userId = _httpContextAccessor.HttpContext?.Items["UserId"]?.ToString();
            var userRole = _httpContextAccessor.HttpContext?.Items["UserRole"]?.ToString();
            var createdBy = string.IsNullOrEmpty(userId) ? "Self" : $"{userRole} - {userId}";

            plan.CreatedAt = DateTime.UtcNow;
            plan.CreatedBy = createdBy;
            await _planRepository.AddAsync(plan);
            await _planRepository.SaveChangesAsync();
            return _mapper.Map<PlanDto>(plan);
        }
        public async Task<PlanDto?> UpdatePlanAsync(int id, UpdatePlanDto updateDto)
        {
            var plan = await _planRepository.GetByIdAsync(id);
            if (plan == null) return null;

            var userId = _httpContextAccessor.HttpContext?.Items["UserId"]?.ToString();
            var userRole = _httpContextAccessor.HttpContext?.Items["UserRole"]?.ToString();
            //var updatedBy = string.IsNullOrEmpty(userId) ? "Self" : $"{userRole} - {userId}";
            var createdBy = string.IsNullOrEmpty(userId) ? "Self" : $"{userRole} - {userId}";

            plan.UpdatedAt = DateTime.UtcNow;
            plan.UpdatedBy = createdBy;
            plan.BillingCycle = Domain.Enum.BillingCycle.Quarterly;

            // Only update if not null, empty, or default-like value
            if (!string.IsNullOrEmpty(updateDto.PlanName) && updateDto.PlanName.ToLower() != "string")
                plan.PlanName = updateDto.PlanName;

            if (!string.IsNullOrEmpty(updateDto.Description) && updateDto.Description.ToLower() != "string")
                plan.Description = updateDto.Description;

            if (updateDto.MinPlantsAllowed.HasValue && updateDto.MinPlantsAllowed.Value != 0)
                plan.MinPlantsAllowed = updateDto.MinPlantsAllowed;

            if (updateDto.MaxPlantsAllowed.HasValue && updateDto.MaxPlantsAllowed.Value != 0)
                plan.MaxPlantsAllowed = updateDto.MaxPlantsAllowed;

            if (updateDto.MonthlyReplacements.HasValue && updateDto.MonthlyReplacements.Value != 0)
                plan.MonthlyReplacements = updateDto.MonthlyReplacements;

            if (updateDto.Price.HasValue && updateDto.Price.Value != 0)
                plan.Price = updateDto.Price;

            _planRepository.Update(plan);
            await _planRepository.SaveChangesAsync();

            return _mapper.Map<PlanDto>(plan);
        }

        public async Task DeletePlanAsync(int id)
        {
            var plan = await _planRepository.GetByIdAsync(id);
            if(plan != null)
            {
                _planRepository.Delete(plan);
                await _planRepository.SaveChangesAsync();
            }
        }
    }
}
