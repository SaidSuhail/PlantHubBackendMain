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
   public class UserPlanService:IUserPlanService
    {
        private readonly IMapper _mapper;
        private readonly IUserPlanRepository _userPlanRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserPlanService(IMapper mapper,IHttpContextAccessor httpContextAccessor, IUserPlanRepository userPlanRepo)
        {
            _mapper = mapper;
            _userPlanRepo = userPlanRepo;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<ApiResponse<UserPlanDto>>SubscribePlan(AddUserPlanDto dto)
        {
            var userPlans = await _userPlanRepo.GetUserPlansByUserId(dto.UserId);
            foreach(var plan in userPlans)
            {
                if(plan.IsActive && plan.EndDate.Date < DateTime.UtcNow.Date)
                {
                    plan.IsActive = false;
                }
            }
            await _userPlanRepo.SaveAsync();

            bool hasActivePlan = await _userPlanRepo.HasActivePlanAsync(dto.UserId);
            if (hasActivePlan)
            {
                return new ApiResponse<UserPlanDto>(false, "User Already Has An Active plan", null, new { error = "Active plan exists" });
            }
           
            var userPlan = _mapper.Map<UserPlan>(dto);
            userPlan.IsActive = true;
            userPlan.StartDate = DateTime.UtcNow.Date;
            userPlan.EndDate = userPlan.StartDate.AddMonths(3);
            var userId = _httpContextAccessor.HttpContext?.Items["UserId"]?.ToString();
            var userRole = _httpContextAccessor.HttpContext?.Items["UserRole"]?.ToString();
            var createdBy = string.IsNullOrEmpty(userId) ? "Self" : $"{userRole} - {userId}";
            userPlan.CreatedAt = DateTime.UtcNow;
            userPlan.CreatedBy = createdBy;
            await _userPlanRepo.AddAsync(userPlan);
            await _userPlanRepo.SaveAsync();

            var result =  _mapper.Map<UserPlanDto>(userPlan);
            //result.StartDate = userPlan.StartDate.ToString("yyyy-MM-dd");
            //result.EndDate = userPlan.EndDate.ToString("yyyy-MM-dd");
            return new ApiResponse<UserPlanDto>(true, "plan subscribed successfully", result);
        }
        public async Task<ApiResponse<string>> UnsubscribePlan(int userId)
        {
            var plan = await _userPlanRepo.GetUserPlansByUserId(userId);
            var activePlan = plan.FirstOrDefault(p => p.IsActive);
            var userIds = _httpContextAccessor.HttpContext?.Items["UserId"]?.ToString();
            var userRole = _httpContextAccessor.HttpContext?.Items["UserRole"]?.ToString();
            var createdBy = string.IsNullOrEmpty(userIds) ? "Self" : $"{userRole} - {userIds}";
            activePlan.UpdatedAt = DateTime.UtcNow;
            activePlan.UpdatedBy = createdBy;
            if (activePlan == null)
                return new ApiResponse<string>(false, "No active plan found to unsubscribe", null);
            activePlan.IsActive = false;
            await _userPlanRepo.SaveAsync();
            return new ApiResponse<string>(true, "successfully unsubscribed from active plan", "unsubscribed");
        }
        public async Task<List<UserPlanDto>> GetUsersPlan(int userId)
        {
            var plans = await _userPlanRepo.GetUserPlansByUserId(userId);
            return _mapper.Map<List<UserPlanDto>>(plans);
        }

    }
}
