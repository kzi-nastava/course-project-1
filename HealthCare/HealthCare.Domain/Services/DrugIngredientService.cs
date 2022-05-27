using HealthCare.Data.Entities;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;

namespace HealthCare.Domain.Services;

public class DrugIngredientService : IDrugIngredientService
{
    public DrugIngredientService()
    {
    }

    public static DrugIngredientDomainModel ParseToModel(DrugIngredient drugIngredient)
    {
        DrugIngredientDomainModel drugIngredientModel = new DrugIngredientDomainModel
        {
            DrugId = drugIngredient.DrugId,
            Amount = drugIngredient.Amount,
            IngredientId = drugIngredient.IngredientId,
            IsDeleted = drugIngredient.IsDeleted
        };

        if (drugIngredient.Ingredient != null)
            drugIngredientModel.Ingredient = IngredientService.ParseToDTO(drugIngredient.Ingredient);

        
        return drugIngredientModel;
    }

    public static DrugIngredient ParseFromModel(DrugIngredientDomainModel drugIngredientModel)
    {
        DrugIngredient drugIngredient = new DrugIngredient
        {
            DrugId = drugIngredientModel.DrugId,
            Amount = drugIngredientModel.Amount,
            IngredientId = drugIngredientModel.IngredientId,
            IsDeleted = drugIngredientModel.IsDeleted
        };
        
        return drugIngredient;
    }

    public Task<IEnumerable<DrugIngredientDomainModel>> GetAll()
    {
        //TODO: Implement this
        throw new NotImplementedException();
    }
}