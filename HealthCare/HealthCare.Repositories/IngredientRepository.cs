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
    public interface IIngredientRepository : IRepository<Ingredient>
    {
        public Task<Ingredient> GetById(decimal id);
    }
    public class IngredientRepository : IIngredientRepository
    {
        private readonly HealthCareContext _healthCareContext;

        public IngredientRepository(HealthCareContext healthCareContext)
        {
            _healthCareContext = healthCareContext;
        }

        public async Task<IEnumerable<Ingredient>> GetAll()
        {
            return await _healthCareContext.Ingridients.Include(x => x.DrugIngredients).ToListAsync();
        }

        public async Task<Ingredient> GetById(decimal id)
        {
            return await _healthCareContext.Ingridients.Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public void Save()
        {
            _healthCareContext.SaveChanges();
        }
    }
}
