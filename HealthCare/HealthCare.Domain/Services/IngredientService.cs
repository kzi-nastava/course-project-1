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
        private IIngredientRepository _ingredientRepository;

        public IngredientService(IIngredientRepository ingredientRepository)
        {
            _ingredientRepository = ingredientRepository;
        }

        public static IngredientDomainModel ParseToModel(Ingredient ingredient)
        {
            IngredientDomainModel ingredientModel = new IngredientDomainModel
            {
                Id = ingredient.Id,
                IsAllergen = ingredient.IsAllergen,
                Name = ingredient.Name
            };

            ingredientModel.DrugIngredients = new List<DrugIngredientDomainModel>();
            if (ingredient.DrugIngredients != null)
                foreach (DrugIngredient drugIngredient in ingredient.DrugIngredients)
                    ingredientModel.DrugIngredients.Add(DrugIngredientService.ParseToModel(drugIngredient));

            return ingredientModel;
        }

        public static Ingredient ParseFromModel(IngredientDomainModel ingredientModel)
        {
            Ingredient ingredient = new Ingredient
            {
                Id = ingredientModel.Id,
                IsAllergen = ingredientModel.IsAllergen,
                Name = ingredientModel.Name
            };
            
            ingredient.DrugIngredients = new List<DrugIngredient>();
            
            if (ingredientModel.DrugIngredients != null)
                foreach (DrugIngredientDomainModel drugIngredientModel in ingredientModel.DrugIngredients)
                    ingredient.DrugIngredients.Add(DrugIngredientService.ParseFromModel(drugIngredientModel));
            
            return ingredient;
        }

        public async Task<IEnumerable<IngredientDomainModel>> GetAll()
        {
            IEnumerable<Ingredient> data = await _ingredientRepository.GetAll();
            if (data == null)
                return new List<IngredientDomainModel>();

            List<IngredientDomainModel> results = new List<IngredientDomainModel>();
            foreach (Ingredient item in data)
            {
                results.Add(ParseToModel(item));
            }

            return results;
        }
    }
}
