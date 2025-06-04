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

        public async Task<bool> ChangeUserRole(ChangeRoleDTO dto)
        {
            var user = await _roleRepository.GetUserByIdAsync(dto.UserId);
            if (user == null)
                return false;
            if (!Enum.TryParse<UserRole>(dto.NewRole, true, out var newRole))
                return false;
            var performedById = _httpContextAccessor.HttpContext?.Items["UserId"]?.ToString();
            var performedByRole = _httpContextAccessor.HttpContext?.Items["UserRole"]?.ToString();
            var performedBy = $"{performedByRole} - {performedById}";

            user.Role = newRole;
            user.UpdatedAt = DateTime.Now;
            user.UpdatedBy = performedBy;

            await _roleRepository.ChangeUserRoleAsync(user, newRole);
            return true;
        }
    }
}
