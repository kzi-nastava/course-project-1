using HealthCare.Data.Context;
using HealthCare.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCare.Repositories
{
    public interface IDaysOffRequestRepository : IRepository<DaysOffRequest>
    {
        public DaysOffRequest Post(DaysOffRequest daysOffRequest);
    }

    public class DaysOffRequesRepository : IDaysOffRequestRepository
    {
        private readonly HealthCareContext _healthCareContext;

        public DaysOffRequesRepository(HealthCareContext healthCareContext)
        {
            _healthCareContext = healthCareContext;
        }

        public async Task<IEnumerable<DaysOffRequest>> GetAll()
        {
            return await _healthCareContext.DaysOffRequests.ToListAsync();
        }

        public DaysOffRequest Post(DaysOffRequest daysOffRequest)
        {
            EntityEntry<DaysOffRequest> result = _healthCareContext.DaysOffRequests.Add(daysOffRequest);
            return result.Entity;
        }

        public void Save()
        {
            _healthCareContext.SaveChanges();
        }
    }
}
