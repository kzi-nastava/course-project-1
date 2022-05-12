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
    public interface IDrugRepository : IRepository<Drug>
    {
        public Task<Drug> GetById(decimal id);
    }

    public class DrugRepository : IDrugRepository
    {
        private readonly HealthCareContext _healthCareContext;

        public DrugRepository(HealthCareContext healthCareContext)
        {
            _healthCareContext = healthCareContext;
        }

        public async Task<IEnumerable<Drug>> GetAll()
        {
            return await _healthCareContext.Drugs
                .Include(x => x.DrugIngredients).ThenInclude(x => x.Ingredient)
                .ToListAsync();
        }

        public async Task<Drug> GetById(decimal id)
        {
            return await _healthCareContext.Drugs
                .Include(x => x.DrugIngredients).ThenInclude(x => x.Ingredient)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
        }

        public void Save()
        {
            _healthCareContext.SaveChanges();
        }
    }
}
