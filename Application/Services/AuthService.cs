using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Google.Apis.Auth;
using System.Numerics;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interface;
using AutoMapper;
using Domain.Enum;
using Domain.Model;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services
{
   public class AuthService:IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(IAuthRepository authRepository,IHttpContextAccessor httpContextAccessor, IConfiguration configuration, IMapper mapper)
        {
            _authRepository = authRepository;
            _configuration = configuration;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
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
            var userId = _httpContextAccessor.HttpContext?.Items["UserId"]?.ToString();
            var userRole = _httpContextAccessor.HttpContext?.Items["UserRole"]?.ToString();

            var createdBy = string.IsNullOrEmpty(userId) ? "Self" : $"{userRole} - {userId}";


            var user = _mapper.Map<User>(userRegisterDto);
            user.Password = BCrypt.Net.BCrypt.HashPassword(userRegisterDto.Password);
            user.Role = UserRole.User;
            user.CreatedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;
            user.CreatedBy = createdBy;
            user.UpdatedBy = createdBy;
            user.loginType = User.LoginType.Local;
            
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
        public async Task<UserResponseDto> GoogleLoginAsync(GoogleLoginDto googleLoginDto)
        {
            GoogleJsonWebSignature.Payload payload;

            try
            {
                Console.WriteLine("Received IdToken: " + googleLoginDto.IdToken); // Log token
                // Add audience check here with your client ID
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string>
            {
                "152368794260-5dcgj32hu86qu469tbgupnoef67ndvjr.apps.googleusercontent.com"
            }
                };

                // Validate token with settings
                payload = await GoogleJsonWebSignature.ValidateAsync(googleLoginDto.IdToken, settings);
                    Console.WriteLine($"Google token validated for email: {payload.Email}");
            }
            catch(Exception ex)
            {
                Console.WriteLine("Google Token Validation Failed: " + ex.Message);
                return new UserResponseDto { Error = "Invalid Google Token" };
            }

            // Check if user exists in DB
            var user = await _authRepository.GetUserByEmail(payload.Email);

            if (user == null)
            {
                user = new User
                {
                    UserEmail = payload.Email,
                    UserName = payload.Name ?? payload.Email,
                    Role = UserRole.User,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    CreatedBy = "Google",
                    UpdatedBy = "Google",
                    loginType = User.LoginType.Google
                };
                await _authRepository.AddUser(user);
            }

            // Generate JWT token
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
        public async Task<UserResponseDto> GoogleLoginOnlyAsync(GoogleLoginDto googleLoginDto)
        {
            GoogleJsonWebSignature.Payload payload;

            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string> { "152368794260-5dcgj32hu86qu469tbgupnoef67ndvjr.apps.googleusercontent.com" }
                };

                payload = await GoogleJsonWebSignature.ValidateAsync(googleLoginDto.IdToken, settings);
            }
            catch (Exception ex)
            {
                return new UserResponseDto { Error = "Invalid Google Token" };
            }

            var user = await _authRepository.GetUserByEmail(payload.Email);

            if (user == null)
                return new UserResponseDto { Error = "User not found. Please register first." };

            if (user.IsBlocked)
                return new UserResponseDto { Error = "User Blocked" };

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
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
