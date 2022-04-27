using HealthCare.Data.Entities;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using HealthCare.Repositories;

namespace HealthCare.Domain.Services;

public class InventoryService : IInventoryService {
    private IInventoryRepository _inventoryRepository;

    public InventoryService(IInventoryRepository inventoryRepository) {
        _inventoryRepository = inventoryRepository;
    }

    public async Task<IEnumerable<InventoryDomainModel>> GetAll()
    {
        IEnumerable<Inventory> inventories = await _inventoryRepository.GetAll();
        if (inventories == null)
            return null;

        List<InventoryDomainModel> results = new List<InventoryDomainModel>();
        InventoryDomainModel inventoryModel;
        foreach (Inventory item in inventories)
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

}