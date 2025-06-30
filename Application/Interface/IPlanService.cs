using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Domain.Model;

namespace Application.Interface
{
   public interface IPlanService
    {
        Task<ApiResponse<IEnumerable<PlanDto>>> GetAllPlansAsync();
        Task<ApiResponse<PlanDto?>> GetPlanByIdAsync(int id);
        Task<ApiResponse<PlanDto?>> CreatePlanAsync(PlanDto planDto);
        Task<ApiResponse<PlanDto?>> UpdatePlanAsync(int id,UpdatePlanDto updateDto);
        Task <ApiResponse<bool>> DeletePlanAsync(int id);
    }
}
