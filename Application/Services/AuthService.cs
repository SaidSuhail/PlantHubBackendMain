using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Numerics;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interface;
using AutoMapper;
using Domain.Enum;
using Domain.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services
{
   public class AuthService:IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public AuthService(IAuthRepository authRepository, IConfiguration configuration, IMapper mapper)
        {
            _authRepository = authRepository;
            _configuration = configuration;
            _mapper = mapper;
        }
        public async Task<bool>Register(UserRegisterDto userRegisterDto)
        {
            var email = userRegisterDto.UserEmail?.Trim();
            var phone = userRegisterDto.Phone?.Trim();
            if (string.IsNullOrWhiteSpace(email) || !IsValidEmail(email))
                return false;
            if (await _authRepository.IsUserExist(email))
                return false;
            var userName = userRegisterDto.UserName?.Trim().ToLower();
            if (await _authRepository.IsUserNameExist(userName))
                return false;
            if (await _authRepository.IsPhoneExist(phone))
                return false;
            userRegisterDto.UserName = userName;
            userRegisterDto.UserEmail = email;
            var user = _mapper.Map<User>(userRegisterDto);
            user.Password = BCrypt.Net.BCrypt.HashPassword(userRegisterDto.Password);
            user.Role = UserRole.User;
            user.CreatedAt = DateTime.Now;
            await _authRepository.AddUser(user);
            return true;
        }
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email && !email.Contains(" ");
            }
            catch
            {
                return false;
            }
        }

        public async Task<UserResponseDto> Login(UserLoginDto userLoginDto)
        {
            var user = await _authRepository.GetUserByEmail(userLoginDto.Email);

            if (user == null) return new UserResponseDto { Error = "Not Found" };
            if (!BCrypt.Net.BCrypt.Verify(userLoginDto.Password, user.Password)) return new UserResponseDto { Error = "Invalid Password" };
            if (user.IsBlocked) return new UserResponseDto { Error = "User Blocked" };

            var token = GenerateToken(user);

            return new UserResponseDto
            {
                UserName = user.UserName,
                Token = token,
                UserEmail = user.UserEmail,
                Role = user.Role.ToString(),
                Id = user.Id
            };
        }

        private string GenerateToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim(ClaimTypes.Email, user.UserEmail)
            };

            var token = new JwtSecurityToken(
                claims: claims,
                signingCredentials: credentials,
                expires: DateTime.UtcNow.AddDays(1)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


    }
}
