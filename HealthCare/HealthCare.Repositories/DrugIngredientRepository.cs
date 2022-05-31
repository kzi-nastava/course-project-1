using HealthCare.Data.Context;
using HealthCare.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCare.Repositories
{
    public interface IDrugIngredientRepository : IRepository<DrugIngredient>
    {

    }

    public class DrugIngredientRepository : IDrugIngredientRepository
    {
        private readonly HealthCareContext _healthCareContext;

        public DrugIngredientRepository(HealthCareContext healthCareContext)
        {
            _healthCareContext = healthCareContext;
        }

        public Task<IEnumerable<DrugIngredient>> GetAll()
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            _healthCareContext.SaveChanges();
        }
    }
}
