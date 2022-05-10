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
    public interface IRenovationRepository : IRepository<Renovation>
    {
        public Renovation Post(Renovation r);
    }
    internal class RenovationRepository : IRenovationRepository
    {
        private readonly HealthCareContext _healthCareContext;

        public RenovationRepository(HealthCareContext healthCareContext)
        {
            _healthCareContext = healthCareContext;
        }

        public async Task<IEnumerable<Renovation>> GetAll()
        {
            return await _healthCareContext.Renovations.ToListAsync();
        }

        public async Task<IEnumerable<Renovation>> GetSimpleRenovations()
        {
            return await _healthCareContext.Renovations.Where(r => r.IsSimple()).ToListAsync();
        }

        public async Task<IEnumerable<Renovation>> GetComplexRenovations()
        {
            return await _healthCareContext.Renovations.Where(r => !r.IsSimple()).ToListAsync();
        }

        public async Task<IEnumerable<Renovation>> GetSplitRenovations()
        {
            return await _healthCareContext.Renovations.Where(r => r.IsSplit()).ToListAsync();
        }

        public async Task<IEnumerable<Renovation>> GetJoinRenovations()
        {
            return await _healthCareContext.Renovations.Where(r => !r.IsSplit()).ToListAsync();
        }

        public Renovation Post(Renovation renovation)
        {
            EntityEntry<Renovation> result = _healthCareContext.Renovations.Add(renovation);
            return result.Entity;
        }

        public void Save()
        {
            _healthCareContext.SaveChanges();
        }
    }
}
