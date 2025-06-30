using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interface;
using Domain.Enum;
using Domain.Model;

namespace Application.Services
{
   public class RoleChangeRequestService:IRoleChangeRequestService
    {
        private readonly IRoleChangeRequestRepository _requestRepo;
        private readonly IRoleRepository _roleRepo;
        private readonly IRoleService _roleservice;

        public RoleChangeRequestService(IRoleChangeRequestRepository requestRepo, IRoleRepository roleRepo, IRoleService roleservice)
        {
            _requestRepo = requestRepo;
            _roleRepo = roleRepo;
            _roleservice = roleservice;
        }
        public async Task<ApiResponse<bool>> SubmitRequestAsync(RequestRoleChangeDTO dto)
        {
            // Check user exists
            var user = await _roleRepo.GetUserByIdAsync(dto.UserId);
            Console.WriteLine($"UserId: {user.Id}, Role: {user.Role}"); // Add this for logging
            if (user == null)
                return new ApiResponse<bool>(false, "User Not Found", false);

            // Validate requested role enum
            if (!Enum.TryParse<UserRole>(dto.RequestedRole ?? "", true, out var requestedRole))
                return new ApiResponse<bool>(false, "Invalid Requested Role", false);

            //// Check if same or higher role
            if (user.Role == requestedRole || user.Role == UserRole.Provider)
                return new ApiResponse<bool>(false, "Already have this role or higher", false);
            if (user.Role == UserRole.Admin)
                return new ApiResponse<bool>(false, "Admins cannot request role change", false);

            // Check for existing pending request of same role
            var pendingRequests = await _requestRepo.GetPendingRequestsAsync();
            var existingRequest = pendingRequests.Find(r => r.UserId == dto.UserId && r.RequestedRole == requestedRole);
            if (existingRequest != null)
                return new ApiResponse<bool>(false, "You already have a pending request for this role", false);

            // Create new request
            var request = new RoleChangeRequest
            {
                UserId = dto.UserId,
                RequestedRole = requestedRole,
                Status = RoleRequestStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = user.UserName
            };

            await _requestRepo.AddRequestAsync(request);

            return new ApiResponse<bool>(true, "Request submitted successfully", true);
        }

        public async Task<ApiResponse<List<RoleChangeRequest>>> GetPendingRequestsAsync()
        {
            var pending = await _requestRepo.GetPendingRequestsAsync();
            return new ApiResponse<List<RoleChangeRequest>>(true, "Pending requests fetched", pending);
        }

        public async Task<ApiResponse<bool>> ReviewRequestAsync(ReviewRoleChangeRequestDTO dto, string reviewer)
        {
            var request = await _requestRepo.GetRequestByIdAsync(dto.RequestId);
            if (request == null)
                return new ApiResponse<bool>(false, "Request not found", false);

            if (request.Status != RoleRequestStatus.Pending)
                return new ApiResponse<bool>(false, "Request already reviewed", false);

            request.Status = dto.Approve ? RoleRequestStatus.Approved : RoleRequestStatus.Rejected;
            request.CreatedAt = DateTime.UtcNow;
            request.CreatedBy = reviewer;

            if (dto.Approve)
            {
                // Change user role using existing RoleService
                var changeDto = new ChangeRoleDTO
                {
                    UserId = request.UserId,
                    NewRole = request.RequestedRole.ToString()
                };

                var changeResponse = await _roleservice.ChangeUserRole(changeDto);
                if (!changeResponse.Success)
                    return new ApiResponse<bool>(false, "Failed to update user role", false);
            }

            await _requestRepo.UpdateRequestAsync(request);

            return new ApiResponse<bool>(true, $"Request {(dto.Approve ? "approved" : "rejected")}", true);
        }
        public async Task<ApiResponse<List<RoleChangeRequest>>> GetAllRequestsAsync()
        {
            var all = await _requestRepo.GetAllRequestsAsync();
            return new ApiResponse<List<RoleChangeRequest>>(true, "All requests fetched", all);
        }


    }
}
