using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthCare.Data.Context;
using HealthCare.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthCare.Repositories {
    public interface IEquipmentRepository : IRepository<Equipment> {

    }
    public class EquipmentRepository : IEquipmentRepository {
        private readonly HealthCareContext _healthCareContext;

        public EquipmentRepository(HealthCareContext healthCareContext) {
            _healthCareContext = healthCareContext;
        }
        public async Task<IEnumerable<Equipment>> GetAll() {
            return await _healthCareContext.Equipments.ToListAsync();
        }
    }
}
