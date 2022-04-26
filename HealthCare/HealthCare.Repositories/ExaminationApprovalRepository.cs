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
        public ExaminationApproval Update(ExaminationApproval approval);
        public Task<ExaminationApproval>  GetExaminationApprovalById(decimal id);
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
        
        public ExaminationApproval Update(ExaminationApproval approval)
        {
            var updatedEntry = _healthCareContext.ExaminationApprovals.Attach(approval);
            _healthCareContext.Entry(approval).State = EntityState.Modified;
            return updatedEntry.Entity;
        }

        public async Task<ExaminationApproval> GetExaminationApprovalById(decimal id)
        {
            var examinationApproval = await _healthCareContext.ExaminationApprovals.FirstOrDefaultAsync(x => x.Id == id);
            return examinationApproval;
        }
    }
}
