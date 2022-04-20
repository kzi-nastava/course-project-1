using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthCare.Data.Context;
using HealthCare.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthCare.Repositories {
    public interface IExaminationRepository : IRepository<Examination> {

    }
    public class ExaminationRepository : IExaminationRepository {
        private readonly HealthCareContext _healthCareContext;

        public ExaminationRepository(HealthCareContext healthCareContext) {
            _healthCareContext = healthCareContext;
        }
        public async Task<IEnumerable<Examination>> GetAll() {
            return await _healthCareContext.Examinations.ToListAsync();
        }
    }
}
