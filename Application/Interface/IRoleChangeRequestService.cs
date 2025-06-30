using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Domain.Model;

namespace Application.Interface
{
   public interface IRoleChangeRequestService
    {
        Task<ApiResponse<bool>> SubmitRequestAsync(RequestRoleChangeDTO dto);
        Task<ApiResponse<List<RoleChangeRequest>>> GetPendingRequestsAsync();
        Task<ApiResponse<bool>> ReviewRequestAsync(ReviewRoleChangeRequestDTO dto, string reviewer);
        Task<ApiResponse<List<RoleChangeRequest>>> GetAllRequestsAsync();

    }
}
