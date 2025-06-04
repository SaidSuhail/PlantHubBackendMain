using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;

namespace Application.Interface
{
   public interface IAuthService
    {
        Task<bool> Register(UserRegisterDto userRegisterDto);
        Task<UserResponseDto> Login(UserLoginDto userLoginDto);
        Task<UserResponseDto> GoogleLoginAsync(GoogleLoginDto googleLoginDto);
        Task<UserResponseDto> GoogleLoginOnlyAsync(GoogleLoginDto googleLoginDto);
    }
}
