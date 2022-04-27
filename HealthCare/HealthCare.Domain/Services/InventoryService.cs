using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using HealthCare.Repositories;

namespace HealthCare.Domain.Services;

public class InventoryService : IInventoryService {
    private IInventoryRepository _inventoryRepository;

    public InventoryService(IInventoryRepository inventoryRepository) {
        _inventoryRepository = inventoryRepository;
    }

    // Async awaits info from database
    // GetAll is the equivalent of SELECT *
    public async Task<IEnumerable<InventoryDomainModel>> GetAll()
    {
        var data = await _inventoryRepository.GetAll();
        if (data == null)
            return null;

        List<InventoryDomainModel> results = new List<InventoryDomainModel>();
        InventoryDomainModel inventoryModel;
        foreach (var item in data)
        {
            inventoryModel = new InventoryDomainModel
            {
                IsDeleted = item.IsDeleted,
                roomId = item.roomId,
                Amount = item.Amount,
                equipmentId = item.equipmentId,
            };
            if (item.Equipment != null) {
                inventoryModel.Equipment = new EquipmentDomainModel {
                    Id = item.Equipment.Id,
                    equipmentTypeId = item.Equipment.equipmentTypeId,
                    IsDeleted = item.Equipment.IsDeleted,
                    Name = item.Equipment.Name,
                };
                if (inventoryModel.Equipment.EquipmentType != null)
                    inventoryModel.Equipment.EquipmentType = new EquipmentTypeDomainModel {
                        Id = item.Equipment.EquipmentType.Id,
                        Name = item.Equipment.EquipmentType.Name,
                        IsDeleted = item.Equipment.EquipmentType.IsDeleted
                    };
            }
            results.Add(inventoryModel);
        }

        return results;
    }    
    public async Task<IEnumerable<InventoryDomainModel>> ReadAll()
    {
        IEnumerable<InventoryDomainModel> inventory = await GetAll();
        List<InventoryDomainModel> result = new List<InventoryDomainModel>();
        foreach (var item in inventory)
        {
            if(!item.IsDeleted) result.Add(item);
        }
        return result;
    }    
}