using HealthCare.Data.Entities;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using HealthCare.Repositories;

namespace HealthCare.Domain.Services;

public class EquipmentTypeService : IEquipmentTypeService 
{
    private IEquipmentTypeRepository _equipmentTypeRepository;

    public EquipmentTypeService(IEquipmentTypeRepository equipmentTypeRepository)
    {
        _equipmentTypeRepository = equipmentTypeRepository;
    }

    public async Task<IEnumerable<EquipmentTypeDomainModel>> GetAll()
    {
        IEnumerable<EquipmentType> equipmentTypes = await _equipmentTypeRepository.GetAll();
        if (equipmentTypes == null)
            return null;

        List<EquipmentTypeDomainModel> results = new List<EquipmentTypeDomainModel>();
        EquipmentTypeDomainModel equipmentTypeModel;
        foreach (EquipmentType item in equipmentTypes)
        {
            equipmentTypeModel = new EquipmentTypeDomainModel
            {
                Id = item.Id,
                IsDeleted = item.IsDeleted,
                Name = item.Name,
            };
            results.Add(equipmentTypeModel);
        }

        return results;
    }
    public async Task<IEnumerable<EquipmentTypeDomainModel>> ReadAll()
    {
        IEnumerable<EquipmentTypeDomainModel> equipmentTypes = await GetAll();
        List<EquipmentTypeDomainModel> result = new List<EquipmentTypeDomainModel>();
        foreach (var item in equipmentTypes)
        {
            if (!item.IsDeleted) result.Add(item);
        }
        return result;
    }
}