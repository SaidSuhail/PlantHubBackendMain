using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;

namespace Application.Interface
{
   public interface IUserPlanService
    {
        //Task<UserPlanDto> SubscribePlan(AddUserPlanDto dto);
        Task<ApiResponse<UserPlanDto>> SubscribePlan(AddUserPlanDto dto);

        Task<List<UserPlanDto>> GetUsersPlan(int userId);
        Task<ApiResponse<string>> UnsubscribePlan(int userPlanId);


    }
}
