using HealthCare.Data.Context;
using HealthCare.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCare.Repositories
{
    public interface IPrescriptionRepository : IRepository<Prescription>
    {

    }
    public class PrescriptionRepository : IPrescriptionRepository
    {
        private readonly HealthCareContext _healthCareContext;

        public PrescriptionRepository(HealthCareContext healthCareContext)
        {
            _healthCareContext = healthCareContext;
        }

        public async Task<IEnumerable<Prescription>> GetAll()
        {
            return await _healthCareContext.Prescriptions
                .Include(x => x.Drug)
                .ToListAsync();
        }

        public void Save()
        {
            _healthCareContext.SaveChanges();
        }
    }
}
