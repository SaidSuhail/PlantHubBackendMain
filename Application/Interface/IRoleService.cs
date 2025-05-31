﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;

namespace Application.Interface
{
    public interface IRoleService
    {
        Task<bool> ChangeUserRole(ChangeRoleDTO dto);
    }
}
