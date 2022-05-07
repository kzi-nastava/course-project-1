using HealthCare.Data.Entities;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using HealthCare.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCare.Domain.Services
{
    public class IngredientService : IIngredientService
    {
        private IIngredientRepository _ingridientRepository;

        public IngredientService(IIngredientRepository ingridientRepository)
        {
            _ingridientRepository = ingridientRepository;
        }

        private IngredientDomainModel parseToModel(Ingredient ingridient)
        {
            IngredientDomainModel ingridientModel = new IngredientDomainModel
            {
                Id = ingridient.Id,
                IsAllergen = ingridient.IsAllergen,
                Name = ingridient.Name
            };
            return ingridientModel;
        }

        private Ingredient parseFromModel(IngredientDomainModel ingridientModel)
        {
            Ingredient ingridient = new Ingredient
            {
                Id = ingridientModel.Id,
                IsAllergen = ingridientModel.IsAllergen,
                Name = ingridientModel.Name
            };
            return ingridient;
        }

        public async Task<IEnumerable<IngredientDomainModel>> GetAll()
        {
            IEnumerable<Ingredient> data = await _ingridientRepository.GetAll();
            if (data == null)
                return new List<IngredientDomainModel>();

            List<IngredientDomainModel> results = new List<IngredientDomainModel>();
            foreach (Ingredient item in data)
            {
                results.Add(parseToModel(item));
            }

            return results;
        }
    }
}
