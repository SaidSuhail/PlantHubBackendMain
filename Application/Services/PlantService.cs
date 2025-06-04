using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interface;
using AutoMapper;
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
        public async Task AddPlant(AddPlantDto plantDto, IFormFile image)
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
            }
            catch(DbUpdateException ex)
            {
                throw new Exception("A Plant with similar data already exist");
            }
            catch(Exception ex)
            {
                throw new Exception("An error occured while Adding Product");
            }
           
        }
        public async Task<List<PlantWithCategoryDto>> GetPlants()
        {
            var plants = await _plantRepo.GetAllPlantsAsync();
            return _mapper.Map<List<PlantWithCategoryDto>>(plants);
        }
        public async Task<PlantWithCategoryDto?>GetPlantById(int id)
        {
            var plant = await _plantRepo.GetPlantByIdAsync(id);
            return plant == null ? null : _mapper.Map<PlantWithCategoryDto>(plant);
        }
        public async Task<List<PlantWithCategoryDto>> GetPlantsByCategory(string categoryName)
        {
            var plants = await _plantRepo.GetPlantsByCategoryAsync(categoryName);
            return _mapper.Map<List<PlantWithCategoryDto>>(plants);
        }
        public async Task<bool>DeletePlant(int id)
        {
            var plant = await _plantRepo.GetPlantByIdAsync(id);
            if (plant == null)
                return false;
            return await _plantRepo.DeletePlantAsync(plant);
        }
   
        public async Task UpdatePlant(int id, UpdatePlantDto plantDto, IFormFile? image)
        {
            var plant = await _plantRepo.GetPlantByIdAsync(id);
            if (plant == null)
                throw new Exception("Plant not found");
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
                    throw new Exception("Category does not exist.");

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
        }



        public async Task<List<PlantWithCategoryDto>>SearchPlant(string search)
        {
            var plant = await _plantRepo.SearchPlantsAsync(search);
            return _mapper.Map<List<PlantWithCategoryDto>>(plant);
        }
        public async Task<PagedResponseDto<PlantViewDto>>GetPaginatedPlants(int pageNumber,int pageSize)
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
            return new PagedResponseDto<PlantViewDto>
            {
                CurrentPage = pageSize,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                Items = plantDtos
            };
        }
    }
}
