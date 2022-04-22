using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthCare.Data.Context;
using HealthCare.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthCare.Repositories {
    public interface IExaminationApprovalRepository : IRepository<ExaminationApproval> {
        public ExaminationApproval Post(ExaminationApproval examinationApproval);
    }
    public class ExaminationApprovalRepository : IExaminationApprovalRepository {
        private readonly HealthCareContext _healthCareContext;

        public ExaminationApprovalRepository(HealthCareContext healthCareContext) {
            _healthCareContext = healthCareContext;
        }
        public async Task<IEnumerable<ExaminationApproval>> GetAll() {
            return await _healthCareContext.ExaminationApprovals.ToListAsync();
        }

        public ExaminationApproval Post(ExaminationApproval examinationApproval) {
            var result = _healthCareContext.Add(examinationApproval);
            return result.Entity;
        }

        public void Save()
        {
            _healthCareContext.SaveChanges();
        }
    }
}
