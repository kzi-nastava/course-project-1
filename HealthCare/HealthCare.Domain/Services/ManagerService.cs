using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using HealthCare.Repositories;

namespace HealthCare.Domain.Services;

public class ManagerService : IManagerService{
    private IManagerRepository _managerRepository;

    public ManagerService(IManagerRepository managerRepository) {
        _managerRepository = managerRepository;
    }

    public Task<IEnumerable<ManagerDomainModel>> GetAll()
    {
        throw new NotImplementedException();
    } 
}