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
        Task<IEnumerable<PlanDto>> GetAllPlansAsync();
        Task<PlanDto?> GetPlanByIdAsync(int id);
        Task<PlanDto?> CreatePlanAsync(PlanDto planDto);
        Task<PlanDto?> UpdatePlanAsync(int id,UpdatePlanDto updateDto);
        Task DeletePlanAsync(int id);
    }
}
