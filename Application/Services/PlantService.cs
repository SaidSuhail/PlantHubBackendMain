using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interface;
using AutoMapper;
using Domain.Enum;
using Domain.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
   public class PlantService:IPlantService
    {
        private readonly IPlantRepository _plantRepo;
        private readonly ICategoryRepository _categoryRepo;
        private readonly IMapper _mapper;
        private readonly ICloudinaryServices _cloudinaryServices;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public PlantService(IPlantRepository plantRepo,IHttpContextAccessor httpContextAccessor, ICategoryRepository categoryRepo, IMapper mapper, ICloudinaryServices cloudinaryServices)
        {
            _plantRepo = plantRepo;
            _categoryRepo = categoryRepo;
            _mapper = mapper;
            _cloudinaryServices = cloudinaryServices;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<ApiResponse<string>> AddPlant(AddPlantDto plantDto, IFormFile image)
        {
            try
            {
            string imageUrl = await _cloudinaryServices.UploadImageAsync(image);
            var plant = _mapper.Map<Plant>(plantDto);
            var userId = _httpContextAccessor.HttpContext?.Items["UserId"]?.ToString();
            var userRole = _httpContextAccessor.HttpContext?.Items["UserRole"]?.ToString();

            var createdBy = string.IsNullOrEmpty(userId) ? "Self" : $"{userRole} - {userId}";

            plant.ImageUrl = imageUrl;
            plant.CreatedAt = DateTime.UtcNow;
            plant.CreatedBy = createdBy;


            await _plantRepo.AddPlantAsync(plant);
                return new ApiResponse<string>(true, "Plant Added Successfully", "success", null);
            }
            catch(DbUpdateException )
            {
                return new ApiResponse<string>(false, "A Plant With Similar Data Already Exists", null, "Duplicate Entry");
            }
            catch(Exception ex)
            {
                return new ApiResponse<string>(false, "An Error Occured While Adding Plant", null, ex.Message);
            }
           
        }
        public async Task<ApiResponse<List<PlantWithCategoryDto>>> GetPlants()
        {
            var plants = await _plantRepo.GetAllPlantsAsync();
            var result =  _mapper.Map<List<PlantWithCategoryDto>>(plants);
            return new ApiResponse<List<PlantWithCategoryDto>>(true, "Fetched All plants", result, null);
        }

        public async Task<ApiResponse<PlantWithCategoryDto?>>GetPlantById(int id)
        {
            var plant = await _plantRepo.GetPlantByIdAsync(id);
            if (plant == null)
                return new ApiResponse<PlantWithCategoryDto?>(false, "Plant Not Found", null);
            return new ApiResponse<PlantWithCategoryDto?>(true, "Plant Retrieved", _mapper.Map<PlantWithCategoryDto>(plant));
        }

        public async Task<ApiResponse<List<PlantWithCategoryDto>>> GetPlantsByCategory(string categoryName)
        {
            var plants = await _plantRepo.GetPlantsByCategoryAsync(categoryName);
            var result =  _mapper.Map<List<PlantWithCategoryDto>>(plants);
            return new ApiResponse<List<PlantWithCategoryDto>>(true, "Plants fetched by category", result);
        }

        public async Task<ApiResponse<bool>>DeletePlant(int id)
        {
            var plant = await _plantRepo.GetPlantByIdAsync(id);
            if (plant == null)
                return new ApiResponse<bool>(false,"Plant Not Found",false);
            var deleted =  await _plantRepo.DeletePlantAsync(plant);
            return new ApiResponse<bool>(true, "Plant Deleted", deleted);
        }
   
        public async Task<ApiResponse<string>> UpdatePlant(int id, UpdatePlantDto plantDto, IFormFile? image)
        {
            try
            {
                var plant = await _plantRepo.GetPlantByIdAsync(id);
                if (plant == null)
                    return new ApiResponse<string>(false, "Plant Not Found", null);

                var userId = _httpContextAccessor.HttpContext?.Items["UserId"]?.ToString();
                var userRole = _httpContextAccessor.HttpContext?.Items["UserRole"]?.ToString();
                var createdBy = string.IsNullOrEmpty(userId) ? "Self" : $"{userRole} - {userId}";

                plant.UpdatedAt = DateTime.UtcNow;
                plant.UpdatedBy = createdBy;

                if (!string.IsNullOrEmpty(plantDto.Name) && plantDto.Name.ToLower() != "string")
                    plant.Name = plantDto.Name;

                if (!string.IsNullOrEmpty(plantDto.LatinName) && plantDto.LatinName.ToLower() != "string")
                    plant.LatinName = plantDto.LatinName;

                if (!string.IsNullOrEmpty(plantDto.Description) && plantDto.Description.ToLower() != "string")
                    plant.Description = plantDto.Description;

                //if (!string.IsNullOrEmpty(plantDto.CareLevel) && plantDto.CareLevel.ToLower() != "string")
                //    plant.CareLevel = plantDto.CareLevel;
                if (!string.IsNullOrEmpty(plantDto.CareLevel) && Enum.TryParse<CareLevel>(plantDto.CareLevel, true, out var parsedCareLevel))
                {
                    plant.CareLevel = parsedCareLevel;
                }

                if (plantDto.Price.HasValue && plantDto.Price.Value != 0)
                    plant.Price = plantDto.Price.Value;

                if (!string.IsNullOrEmpty(plantDto.Color) && plantDto.Color.ToLower() != "string")
                    plant.Color = plantDto.Color;

                if (plantDto.Stock.HasValue && plantDto.Stock.Value != 0)
                    plant.Stock = plantDto.Stock.Value;

                if (plantDto.CategoryId.HasValue && plantDto.CategoryId.Value != 0)
                {
                    var categoryExists = await _categoryRepo.CategoryExistsAsync(plantDto.CategoryId.Value);
                    if (!categoryExists)
                        return new ApiResponse<string>(false, "category doesnot exist", null);
                    plant.CategoryId = plantDto.CategoryId.Value;
                }

                if (plantDto.ProviderId.HasValue && plantDto.ProviderId.Value != 0)
                {
                    // You might want to validate provider as well if needed
                    plant.ProviderId = plantDto.ProviderId.Value;
                }

                if (image != null && image.Length > 0)
                {
                    string imageUrl = await _cloudinaryServices.UploadImageAsync(image);
                    plant.ImageUrl = imageUrl;
                }

                await _plantRepo.UpdatePlantAsync(plant);
                return new ApiResponse<string>(true, "Plant Updated Successfully", "success");
            }
            catch(Exception ex)
            {
                return new ApiResponse<string>(false, "Failed To Update Plant", null, ex.Message);
            }
          
        }



        public async Task<ApiResponse<List<PlantWithCategoryDto>>>SearchPlant(string search)
        {
            var plant = await _plantRepo.SearchPlantsAsync(search);
            return new ApiResponse<List<PlantWithCategoryDto>>(true,"Search Result", _mapper.Map<List<PlantWithCategoryDto>>(plant));
        }

        public async Task<ApiResponse<PagedResponseDto<PlantViewDto>>>GetPaginatedPlants(int pageNumber,int pageSize)
        {
            var totalCount = await _plantRepo.GetTotalPlantCountAsync();
            var plants = await _plantRepo.GetPaginatedPlantAsync(pageNumber, pageSize);
            var plantDtos = plants.Select(p => new PlantViewDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Color = p.Color,
                LatinName = p.LatinName,
                ImageUrl = p.ImageUrl,
                Stock = p.Stock
            }).ToList();

            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            var response =  new PagedResponseDto<PlantViewDto>
            {
                CurrentPage = pageSize,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                Items = plantDtos
            };
            return new ApiResponse<PagedResponseDto<PlantViewDto>>(true, "Paginated Result", response);
        }
    }
}
