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
        public Doctor Post(Doctor doctor);
        public Doctor Update(Doctor doctor);
        public Doctor Delete(Doctor doctor);
        public Task<Doctor> GetDoctortById(decimal id);
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

        public Doctor Delete(Doctor doctor)
        {
            var deletedDoctor = Update(doctor);
            return deletedDoctor;
        }

        public async Task<Doctor> GetDoctortById(decimal id)
        {
            return await _healthCareContext.Doctors.FirstAsync(x => x.Id == id);
        }

        public Doctor Post(Doctor doctor)
        {
            var result = _healthCareContext.Doctors.Add(doctor);
            return result.Entity;
        }

        public Doctor Update(Doctor doctor)
        {
            var updatedEntry = _healthCareContext.Doctors.Attach(doctor);
            _healthCareContext.Entry(doctor).State = EntityState.Modified;
            return updatedEntry.Entity;
        }
    }
}
