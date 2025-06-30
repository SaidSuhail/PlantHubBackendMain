using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interface;
using AutoMapper;
using Microsoft.AspNetCore.Http;

namespace Application.Services
{
   public class ProfileService:IProfileService
    {
        private readonly IProfileRepository _profileRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly ICloudinaryServices _cloudinary;

        public ProfileService(IProfileRepository profileRepo,IHttpContextAccessor httpContextAccessor,IMapper mapper,ICloudinaryServices cloudinary)
        {
            _profileRepo = profileRepo;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _cloudinary = cloudinary;
        }
        public async Task<ApiResponse<UserProfileDto>> GetProfile()
        {
            var userIdStr = _httpContextAccessor.HttpContext?.Items["UserId"]?.ToString();
            if (!int.TryParse(userIdStr, out int userId))
                return new ApiResponse<UserProfileDto>(false, "Invalid user id", null);
            var user = await _profileRepo.GetUserByIdAsync(userId);
            if (user == null)
                return new ApiResponse<UserProfileDto>(false, "User not found", null);
            var result = _mapper.Map<UserProfileDto>(user);
            return new ApiResponse<UserProfileDto>(true, "Profile fetched", result);
        }
        public async Task<ApiResponse<string>>UpdateProfile(UpdateUserprofileDto dto)
        {
            var userIdStr = _httpContextAccessor.HttpContext?.Items["UserId"]?.ToString();
            if (!int.TryParse(userIdStr, out int userId))
                return new ApiResponse<string>(false, "invalid user id", null);
            var user = await _profileRepo.GetUserByIdAsync(userId);
            if (user == null)
                return new ApiResponse<string>(false, "user not found", null);

            if (!string.IsNullOrWhiteSpace(dto.UserName)&&dto.UserName != "string")
                user.UserName = dto.UserName;

            if (!string.IsNullOrWhiteSpace(dto.Phone)&&dto.Phone != "string")
                user.Phone = dto.Phone;

            if (dto.ProfileImage != null)
                user.ProfileImage = await _cloudinary.UploadImageAsync(dto.ProfileImage);

            var updated = await _profileRepo.UpdateUserAsync(user);
            if (!updated)
                return new ApiResponse<string>(false, "Profile update failed", null);
            return new ApiResponse<string>(true, "Profile updated successfully", "success");
        }

    }
}
