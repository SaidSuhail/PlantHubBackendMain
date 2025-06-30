using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Model;

namespace Application.Interface
{
   public interface IRoleChangeRequestRepository
    {
        Task AddRequestAsync(RoleChangeRequest request);
        Task<RoleChangeRequest?> GetRequestByIdAsync(int id);
        Task<List<RoleChangeRequest>> GetPendingRequestsAsync();
        Task UpdateRequestAsync(RoleChangeRequest request);
        Task<List<RoleChangeRequest>> GetAllRequestsAsync();

    }
}
