using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthCare.Data.Context;
using HealthCare.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthCare.Repositories {
    public interface ITransferRepository : IRepository<Transfer> {

    }
    public class TransferRepository : ITransferRepository {
        private readonly HealthCareContext _healthCareContext;

        public TransferRepository(HealthCareContext healthCareContext) {
            _healthCareContext = healthCareContext;
        }
        public async Task<IEnumerable<Transfer>> GetAll() {
            return await _healthCareContext.Transfers.ToListAsync();
        }
    }
}
