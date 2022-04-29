using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthCare.Data.Context;
using HealthCare.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthCare.Repositories 
{
    public interface IManagerRepository : IRepository<Manager> {
    }
    public class ManagerRepository : IManagerRepository 
    {
        private readonly HealthCareContext _healthCareContext;

        public ManagerRepository(HealthCareContext healthCareContext) 
        {
            _healthCareContext = healthCareContext;
        }
        public async Task<IEnumerable<Manager>> GetAll() 
        {
            return await _healthCareContext.Managers
                .Include(x => x.Credentials)
                .ToListAsync();
        }

        public void Save()
        {
            _healthCareContext.SaveChanges();
        }
    }
}
