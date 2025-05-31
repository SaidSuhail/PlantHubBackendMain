using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interface;
using Domain.Enum;

namespace Application.Services
{
    public class RoleService:IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        
        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<bool> ChangeUserRole(ChangeRoleDTO dto)
        {
            var user = await _roleRepository.GetUserByIdAsync(dto.UserId);
            if (user == null)
                return false;
            if (!Enum.TryParse<UserRole>(dto.NewRole, true, out var newRole))
                return false;
            await _roleRepository.ChangeUserRoleAsync(user, newRole);
            return true;
        }
    }
}
