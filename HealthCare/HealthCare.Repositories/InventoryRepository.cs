using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthCare.Data.Context;
using HealthCare.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthCare.Repositories {
    public interface IInventoryRepository : IRepository<Inventory>
    {
        public Task<Inventory> GetInventoryById(decimal roomId, decimal equipmentId);
        public Inventory Update(Inventory newInventory);
        public Inventory Post(Inventory newInventory);
    }
    public class InventioryRepository : IInventoryRepository {
        private readonly HealthCareContext _healthCareContext;

        public InventioryRepository(HealthCareContext healthCareContext) {
            _healthCareContext = healthCareContext;
        }
        public async Task<IEnumerable<Inventory>> GetAll() {
            return await _healthCareContext.Inventories
                .Include(x => x.Equipment)
                .ToListAsync();
        }

        public void Save()
        {
            _healthCareContext.SaveChanges();
        }
        public async Task<Inventory> GetInventoryById(decimal roomId, decimal equipmentId)
        {
            return await _healthCareContext.Inventories.FindAsync(roomId, equipmentId);
        }

        public Inventory Update(Inventory updatedInventory)
        {
            var updatedEntry = _healthCareContext.Attach(updatedInventory);
            _healthCareContext.Entry(updatedInventory).State = EntityState.Modified;
            return updatedEntry.Entity;
        }

        public Inventory Post(Inventory newInventory)
        {
            var result = _healthCareContext.Inventories.Add(newInventory);
            return result.Entity;
        }
    }
}
