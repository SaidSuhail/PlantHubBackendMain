using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Model;

namespace Application.Interface
{
   public interface IUserPlanRepository
    {
        Task<List<UserPlan>> GetUserPlansByUserId(int userId);
        Task AddAsync(UserPlan userPlan);
        Task<bool> HasActivePlanAsync(int userId);
        Task SaveAsync();
    }
}
