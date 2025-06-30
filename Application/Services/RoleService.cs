using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interface;
using Domain.Enum;
using Microsoft.AspNetCore.Http;

namespace Application.Services
{
    public class RoleService:IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        
        public RoleService(IRoleRepository roleRepository,IHttpContextAccessor httpContextAccessor)
        {
            _roleRepository = roleRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ApiResponse<bool>> ChangeUserRole(ChangeRoleDTO dto)
        {
            try
            {
                    var user = await _roleRepository.GetUserByIdAsync(dto.UserId);
                    if (user == null)
                    return new ApiResponse<bool>(false, "User Not Found", false);

                if (!Enum.TryParse<UserRole>(dto.NewRole, true, out var newRole))
                    return new ApiResponse<bool>(false, "Invalid Role Provider", false);
                    var performedById = _httpContextAccessor.HttpContext?.Items["UserId"]?.ToString();
                    var performedByRole = _httpContextAccessor.HttpContext?.Items["UserRole"]?.ToString();
                    var performedBy = $"{performedByRole} - {performedById}";

                    user.Role = newRole;
                    user.UpdatedAt = DateTime.Now;
                    user.UpdatedBy = performedBy;

                    await _roleRepository.ChangeUserRoleAsync(user, newRole);
                return new ApiResponse<bool>(true, "User Role Updated Successfully",true);
            }catch(Exception ex)
            {
                return new ApiResponse<bool>(false, "Error Occured While Changing Role",false,ex.Message);
            }
          
        }
        public async Task<ApiResponse<List<ProviderDto>>> GetAllProvidersAsync()
        {
            var providers = await _roleRepository.GetAllProvidersAsync();

            var result = providers.Select(p => new ProviderDto
            {
                Id = p.Id,
                UserId = p.UserId,
                ProviderName = p.ProviderName,
                ContactInfo = p.ContactInfo,
                UserName = p.User?.UserName,
                UserEmail = p.User?.UserEmail
            }).ToList();

            return new ApiResponse<List<ProviderDto>>(true, "Providers fetched successfully", result);
        }
    }
}
