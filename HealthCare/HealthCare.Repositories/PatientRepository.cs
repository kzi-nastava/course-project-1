using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthCare.Data.Context;
using HealthCare.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthCare.Repositories {
    public interface IPatientRepository : IRepository<Patient> {

    }
    public class PatientRepository : IPatientRepository {
        private readonly HealthCareContext _healthCareContext;

        public PatientRepository(HealthCareContext healthCareContext) {
            _healthCareContext = healthCareContext;
        }
        public async Task<IEnumerable<Patient>> GetAll() {
            return await _healthCareContext.Patients.ToListAsync();
        }
    }
}
