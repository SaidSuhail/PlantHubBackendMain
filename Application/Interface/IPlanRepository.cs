using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Model;

namespace Application.Interface
{
   public interface IPlanRepository
    {
        Task<IEnumerable<Plan>> GetAllAsync();
        Task<Plan> GetByIdAsync(int id);
        Task AddAsync(Plan plan);
        void Update(Plan plan);
        void Delete(Plan plan);
        Task SaveChangesAsync();
    }
}
