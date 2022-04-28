using HealthCare.Domain.Models;

namespace HealthCare.Domain.Interfaces;

public interface IManagerService : IService<ManagerDomainModel>
{
    public Task<IEnumerable<ManagerDomainModel>> ReadAll();
}