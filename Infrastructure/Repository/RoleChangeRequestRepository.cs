using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interface;
using Domain.Model;
using Infrastructure.Context;
//using Infrastructure.Migrations;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
   public class RoleChangeRequestRepository:IRoleChangeRequestRepository
    {
        private readonly AppdbContext _context;
        public RoleChangeRequestRepository(AppdbContext context)
        {
            _context = context;
        }
        public async Task AddRequestAsync(RoleChangeRequest request)
        {
            await _context.RoleChangeRequests.AddAsync(request);
            await _context.SaveChangesAsync();
        }
        public async Task<RoleChangeRequest?> GetRequestByIdAsync(int id)
        {
            return await _context.RoleChangeRequests
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);
        }
        public async Task<List<RoleChangeRequest>> GetPendingRequestsAsync()
        {
            return await _context.RoleChangeRequests
                .Include(r => r.User)
                .Where(r => r.Status == Domain.Enum.RoleRequestStatus.Pending)
                .ToListAsync();
        }

        public async Task UpdateRequestAsync(RoleChangeRequest request)
        {
            _context.RoleChangeRequests.Update(request);
            await _context.SaveChangesAsync();
        }
        public async Task<List<RoleChangeRequest>> GetAllRequestsAsync()
        {
            return await _context.RoleChangeRequests
                .Include(r => r.User)
                .ToListAsync();
        }

    }
}
