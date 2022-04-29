using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthCare.Data.Context;
using HealthCare.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace HealthCare.Repositories {
    public interface ITransferRepository : IRepository<Transfer> {
        public Transfer Post(Transfer newTransfer);
        public Task<Transfer> GetTransferById(decimal id);
    }
    public class TransferRepository : ITransferRepository {
        private readonly HealthCareContext _healthCareContext;

        public TransferRepository(HealthCareContext healthCareContext) {
            _healthCareContext = healthCareContext;
        }
        public async Task<IEnumerable<Transfer>> GetAll() {
            return await _healthCareContext.Transfers
                .Include(x => x.Equipment).ThenInclude(x => x.EquipmentType)
                .ToListAsync();
        }
        public Transfer Post(Transfer newTransfer)
        {
            EntityEntry<Transfer> result = _healthCareContext.Add(newTransfer);
            return result.Entity;
        }
        public void Save()
        {
            _healthCareContext.SaveChanges();
        }
        public async Task<Transfer> GetTransferById(decimal id)
        {
            return await _healthCareContext.Transfers.FindAsync(id);
        }
    }
}
