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
        public async Task<ApiResponse<IEnumerable<PlanDto>>> GetAllPlansAsync()
        {
            try
            {
                 var plans = await _planRepository.GetAllAsync();
                 var planDtos = _mapper.Map<IEnumerable<PlanDto>>(plans);
                 return new ApiResponse<IEnumerable<PlanDto>>(true, "Plans Retrieved Successfully", planDtos, null);
            }
            catch(Exception ex)
            {
                return new ApiResponse<IEnumerable<PlanDto>>(false, "Failed to retrieve plans", null, ex.Message);
            }
       
        }
        public async Task<ApiResponse<PlanDto?>>GetPlanByIdAsync(int id)
        {
            try
            {
                  var plan = await _planRepository.GetByIdAsync(id);
                if (plan == null)
                    return new ApiResponse<PlanDto?>(false, "Plan not found", null, null);
                var planDto =  _mapper.Map<PlanDto>(plan);
                return new ApiResponse<PlanDto?>(true, "Plan Retrieved Successfully", planDto, null);
            }
            catch(Exception ex)
            {
                return new ApiResponse<PlanDto?>(false, "Failed To Retrieve Plans", null, ex.Message);
            }
            
        }
        public async Task<ApiResponse<PlanDto?>> CreatePlanAsync(PlanDto planDto)
        {
            try
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

                var createPlanDto =  _mapper.Map<PlanDto>(plan);
                return new ApiResponse<PlanDto?>(true, "Plan Created Successfully", createPlanDto, null);
            }
            catch(Exception ex)
            {
                return new ApiResponse<PlanDto?>(false, "Failed To Create Plan", null, ex.Message);
            }
            
        }
        public async Task<ApiResponse<PlanDto?>> UpdatePlanAsync(int id, UpdatePlanDto updateDto)
        {
            try
            {
                var plan = await _planRepository.GetByIdAsync(id);
                if (plan == null)
                    return new ApiResponse<PlanDto?>(false, "plan not found", null, null);

                var userId = _httpContextAccessor.HttpContext?.Items["UserId"]?.ToString();
                var userRole = _httpContextAccessor.HttpContext?.Items["UserRole"]?.ToString();
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

                var updatedPlanDto = _mapper.Map<PlanDto>(plan);
                return new ApiResponse<PlanDto?>(true, "Plan Updated Successfully", updatedPlanDto, null);
            }
            catch(Exception ex)
            {
                return new ApiResponse<PlanDto?>(false, "Failed To Update Plan", null, ex.Message);
            }
        }

        public async Task<ApiResponse<bool>> DeletePlanAsync(int id)
        {
            try
            {
                var plan = await _planRepository.GetByIdAsync(id);
                if (plan == null)
                    return new ApiResponse<bool>(false, "plan not found", false, null);
              
                    _planRepository.Delete(plan);
                    await _planRepository.SaveChangesAsync();
                return new ApiResponse<bool>(true, "Plan Deleted Successfully", true, null);
            }
            catch(Exception ex)
            {
                return new ApiResponse<bool>(false, "Failed To Delete Plan", false, ex.Message);
            }
           
        }
    }
}
