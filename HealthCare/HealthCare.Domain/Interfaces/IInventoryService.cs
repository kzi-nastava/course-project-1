using HealthCare.Domain.Models;

namespace HealthCare.Domain.Interfaces;

public interface IInventoryService : IService<InventoryDomainModel> 
{
    public Task<IEnumerable<InventoryDomainModel>> ReadAll();
}