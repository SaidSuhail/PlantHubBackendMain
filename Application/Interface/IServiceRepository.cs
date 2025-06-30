using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Model;

namespace Application.Interface
{
   public interface IServiceRepository
    {
        Task<IEnumerable<Service>> GetAllAsync();
        Task<Service> GetByIdAsync(int id);
        Task<Service> AddAsync(Service service);
        Task<List<Service>> GetServicesByIdsAsync(List<int> ids);

        Task<Service> UpdateAsync(Service service);
        Task<bool> DeleteAsync(int id);

    }
}
