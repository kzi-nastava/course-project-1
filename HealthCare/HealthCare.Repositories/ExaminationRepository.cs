﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthCare.Data.Context;
using HealthCare.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthCare.Repositories {
    public interface IExaminationRepository : IRepository<Examination> {
        public Task<IEnumerable<Examination>> GetAllByPatientId(decimal id);
        public Examination Update(Examination examination);
        public Task<Examination> GetExamination(decimal roomId, decimal doctorId, decimal patientId, DateTime StartTime);
    }
    public class ExaminationRepository : IExaminationRepository {
        private readonly HealthCareContext _healthCareContext;

        public ExaminationRepository(HealthCareContext healthCareContext) {
            _healthCareContext = healthCareContext;
        }
        public async Task<IEnumerable<Examination>> GetAll() {
            return await _healthCareContext.Examinations.Include(x => x.Anamnesis).ToListAsync();
        }

        public async Task<IEnumerable<Examination>> GetAllByPatientId(decimal id) {
            return await _healthCareContext.Examinations
                .Include(x => x.Anamnesis)
                .Where(x => x.patientId == id)
                .ToListAsync();
        }

        public Examination Update(Examination examination) {
            var updatedEntry = _healthCareContext.Examinations.Attach(examination);
            _healthCareContext.Entry(examination).State = EntityState.Modified;
            return updatedEntry.Entity;
        }
        public async Task<Examination> GetExamination(decimal roomId, decimal doctorId, decimal patientId, DateTime StartTime) {
            return await _healthCareContext.Examinations
                .Include(x => x.Anamnesis)
                .Where(x => x.roomId == roomId)
                .Where(x => x.patientId == patientId)
                .Where(x => x.doctorId == doctorId)
                .Where(x => x.StartTime == StartTime)
                .FirstOrDefaultAsync();
        }

        public void Save()
        {
            _healthCareContext.SaveChanges();
        }

        
    }
}
