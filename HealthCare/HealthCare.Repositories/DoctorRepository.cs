using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthCare.Data.Context;
using HealthCare.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthCare.Repositories {
    public interface IDoctorRepository : IRepository<Doctor> {

    }
    public class DoctorRepository : IDoctorRepository {
        private readonly HealthCareContext _healthCareContext;

        public DoctorRepository(HealthCareContext healthCareContext) {
            _healthCareContext = healthCareContext;
        }
        public async Task<IEnumerable<Doctor>> GetAll() {
            return await _healthCareContext.Doctors.ToListAsync();
        }
    }
}
