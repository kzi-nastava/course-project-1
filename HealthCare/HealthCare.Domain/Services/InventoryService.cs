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
                Equipment = item.Equipment,
                Room = item.Room
            };
            results.Add(inventoryModel);
        }

        return results;
    }    
}