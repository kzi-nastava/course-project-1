using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using HealthCare.Repositories;

namespace HealthCare.Domain.Services;

public class ManagerService : IManagerService{
    private IManagerRepository _managerRepository;

    public ManagerService(IManagerRepository managerRepository) {
        _managerRepository = managerRepository;
    }

    // Async awaits info from database
    // GetAll is the equivalent of SELECT *
    public async Task<IEnumerable<ManagerDomainModel>> GetAll()
    {
        var data = await _managerRepository.GetAll();
        if (data == null)
            return null;

        List<ManagerDomainModel> results = new List<ManagerDomainModel>();
        ManagerDomainModel managerModel;
        foreach (var item in data)
        {
            managerModel = new ManagerDomainModel
            {
                isDeleted = item.isDeleted,
                BirthDate = item.BirthDate,
                Credentials = item.Credentials,
                Email = item.Email,
                Id = item.Id,
                Name = item.Name,
                Phone = item.Phone,
                Surname = item.Surname
            };
            results.Add(managerModel);
        }

        return results;
    } 
}