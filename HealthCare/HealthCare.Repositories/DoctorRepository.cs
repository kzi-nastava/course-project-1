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
            return await _healthCareContext.Doctors
                .Include(x => x.Credentials).ThenInclude(x => x.UserRole)
                .Include(x => x.Examinations).ThenInclude(x => x.Anamnesis)
                .Include(x => x.Operations)
                .ToListAsync();
        }

        public void Save()
        {
            _healthCareContext.SaveChanges();
        }
    }
}
