using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Model;

namespace Application.Interface
{
   public interface ICategoryRepository
    {
        Task<List<Category>> GetAll();
        Task<Category?> GetByName(string name);
        Task Add(Category category);
        Task SaveChanges();

        Task<Category?> GetById(int id);
        Task<bool> CategoryExistsAsync(int categoryId);

        Task Delete(Category category);
        Task<List<Plant>> GetPlantsByCategoryId(int CategoryId);
        void RemoveProducts(List<Plant> products); // NEW
        void RemoveCategory(Category category);    // NEW, alias for Delete if needed

    }
}
