using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using HealthCare.Repositories;

namespace HealthCare.Domain.Services;

public class EquipmentTypeService : IEquipmentTypeService {
    private IEquipmentTypeRepository _equipmentTypeRepository;

    public EquipmentTypeService(IEquipmentTypeRepository equipmentTypeRepository)
    {
        _equipmentTypeRepository = equipmentTypeRepository;
    }

    // Async awaits info from database
    // GetAll is the equivalent of SELECT *
    public async Task<IEnumerable<EquipmentTypeDomainModel>> GetAll()
    {
        var data = await _equipmentTypeRepository.GetAll();
        if (data == null)
            return null;

        List<EquipmentTypeDomainModel> results = new List<EquipmentTypeDomainModel>();
        EquipmentTypeDomainModel equipmentTypeModel;
        foreach (var item in data)
        {
            equipmentTypeModel = new EquipmentTypeDomainModel
            {
                Id = item.Id,
                IsDeleted = item.IsDeleted,
                Name = item.Name,
                Equipments = item.Equipments
            };
            results.Add(equipmentTypeModel);
        }

        return results;
    }
}