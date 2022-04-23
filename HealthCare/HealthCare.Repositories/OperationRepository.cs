using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthCare.Data.Context;
using HealthCare.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthCare.Repositories {
    public interface IOperationRepository : IRepository<Operation> {
        public Task<IEnumerable<Operation>> GetAllByDoctorId(decimal id);
        public Task<IEnumerable<Operation>> GetAllByPatientId(decimal id);
    }
    public class OperationRepository : IOperationRepository {
        private readonly HealthCareContext _healthCareContext;

        public OperationRepository(HealthCareContext healthCareContext) {
            _healthCareContext = healthCareContext;
        }
        public async Task<IEnumerable<Operation>> GetAll() {
            return await _healthCareContext.Operations.ToListAsync();
        }

        public async Task<IEnumerable<Operation>> GetAllByDoctorId(decimal id) {
            return await _healthCareContext.Operations
                .Where(x => x.DoctorId == id)
                .Where(x => x.isDeleted == false)
                .ToListAsync();
        }

        public async Task<IEnumerable<Operation>> GetAllByPatientId(decimal id)
        {
            return await _healthCareContext.Operations
                .Where(x => x.PatientId == id)
                .Where(x => x.isDeleted == false)
                .ToListAsync();
        }

        public void Save()
        {
            _healthCareContext.SaveChanges();
        }
    }
}
