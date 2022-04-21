using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthCare.Data.Context;
using HealthCare.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthCare.Repositories {
    public interface IMedicalRecordRepository : IRepository<MedicalRecord> {

    }
    public class MedicalRecordRepository : IMedicalRecordRepository {
        private readonly HealthCareContext _healthCareContext;

        public MedicalRecordRepository(HealthCareContext healthCareContext) {
            _healthCareContext = healthCareContext;
        }
        public async Task<IEnumerable<MedicalRecord>> GetAll() {
            return await _healthCareContext.MedicalRecords.ToListAsync();
        }

        public void Save()
        {
            _healthCareContext.SaveChanges();
        }
    }
}
