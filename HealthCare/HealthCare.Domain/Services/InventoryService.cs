using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using HealthCare.Repositories;

namespace HealthCare.Domain.Services;

public class InventoryService : IInventoryService {
    private IInventoryRepository _inventoryRepository;

    public InventoryService(IInventoryRepository inventoryRepository) {
        _inventoryRepository = inventoryRepository;
    }

    public Task<IEnumerable<InventoryDomainModel>> GetAll()
    {
        throw new NotImplementedException();
    }    
}