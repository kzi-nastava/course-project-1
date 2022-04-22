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
                Email = item.Email,
                Id = item.Id,
                Name = item.Name,
                Phone = item.Phone,
                Surname = item.Surname
            };
            if (item.Credentials != null) {
                managerModel.Credentials = new CredentialsDomainModel {
                    Id = item.Credentials.Id,
                    Username = item.Credentials.Username,
                    Password = item.Credentials.Password,
                    doctorId = item.Credentials.doctorId,
                    secretaryId = item.Credentials.secretaryId,
                    managerId = item.Credentials.managerId,
                    patientId = item.Credentials.patientId,
                    userRoleId = item.Credentials.userRoleId

                };
                if (item.Credentials.UserRole != null) {
                    managerModel.Credentials.UserRole = new UserRoleDomainModel {
                        Id = item.Credentials.UserRole.Id,
                        RoleName = item.Credentials.UserRole.RoleName,
                        isDeleted = item.Credentials.UserRole.isDeleted
                    };
                }
            }
            results.Add(managerModel);
        }

        return results;
    } 
}