using HealthCare.Domain.DTOs;
using HealthCare.Domain.Models;

namespace HealthCare.Domain.Interfaces;

public interface IDrugIngredientService : IService<DrugIngredientDomainModel>
{
    public DrugIngredientDomainModel Create(DrugIngredientDTO dto);
    DrugIngredientDomainModel Delete(decimal id);
    DrugIngredientDomainModel Update(DrugIngredientDTO drugIngredientDTO);
}