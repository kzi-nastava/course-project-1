using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthCare.Data.Context;
using HealthCare.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthCare.Repositories {
    public interface IInventoryRepository : IRepository<Inventory> {

    }
    public class InventioryRepository : IInventoryRepository {
        private readonly HealthCareContext _healthCareContext;

        public InventioryRepository(HealthCareContext healthCareContext) {
            _healthCareContext = healthCareContext;
        }
        public async Task<IEnumerable<Inventory>> GetAll() {
            return await _healthCareContext.Inventories.ToListAsync();
        }

        public void Save()
        {
            _healthCareContext.SaveChanges();
        }
    }
}
