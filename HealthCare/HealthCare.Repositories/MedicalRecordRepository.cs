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
        public MedicalRecord Post(MedicalRecord medicalRecord);
        public Task<MedicalRecord> GetByPatientId(decimal patientId);
        public MedicalRecord Update(MedicalRecord medicalRecord);

    }
    public class MedicalRecordRepository : IMedicalRecordRepository {
        private readonly HealthCareContext _healthCareContext;

        public MedicalRecordRepository(HealthCareContext healthCareContext) {
            _healthCareContext = healthCareContext;
        }
        public async Task<IEnumerable<MedicalRecord>> GetAll() {
            return await _healthCareContext.MedicalRecords.ToListAsync();
        }

        public async Task<MedicalRecord> GetByPatientId(decimal patientId) {
            return await _healthCareContext.MedicalRecords
                .Where(x => x.PatientId == patientId)
                .FirstOrDefaultAsync();
        }

        public MedicalRecord Post(MedicalRecord medicalRecord) {
            var result = _healthCareContext.Add(medicalRecord);
            return result.Entity;
        }

        public MedicalRecord Update(MedicalRecord medicalRecord) {
            var updatedEntry = _healthCareContext.MedicalRecords.Attach(medicalRecord);
            _healthCareContext.Entry(medicalRecord).State = EntityState.Modified;
            return updatedEntry.Entity;
        }

        public void Save()
        {
            _healthCareContext.SaveChanges();
        }
    }
}
