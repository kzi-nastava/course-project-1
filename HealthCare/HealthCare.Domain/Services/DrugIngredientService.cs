using HealthCare.Data.Entities;
using HealthCare.Domain.Models;

namespace HealthCare.Domain.Services;

public class DrugIngredientService
{
    public static DrugIngredientDomainModel parseToModel(DrugIngredient drugIngredient)
    {
        DrugIngredientDomainModel drugIngredientModel = new DrugIngredientDomainModel
        {
            DrugId = drugIngredient.DrugId,
            Amount = drugIngredient.Amount,
            IngredientId = drugIngredient.IngredientId
        };
        
        return drugIngredientModel;
    }

    public static DrugIngredient parseFromModel(DrugIngredientDomainModel drugIngredientModel)
    {
        DrugIngredient drugIngredient = new DrugIngredient
        {
            DrugId = drugIngredientModel.DrugId,
            Amount = drugIngredientModel.Amount,
            IngredientId = drugIngredientModel.IngredientId
        };
        
        return drugIngredient;
    }
}