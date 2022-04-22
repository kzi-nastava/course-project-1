using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using HealthCare.Repositories;

namespace HealthCare.Domain.Services;

public class SecretaryService : ISecretaryService{
    private ISecretaryRepository _secretaryRepository;

    public SecretaryService(ISecretaryRepository secretaryRepository) {
        _secretaryRepository = secretaryRepository;
    }

    // Async awaits info from database
    // GetAll is the equivalent of SELECT *
    public async Task<IEnumerable<SecretaryDomainModel>> GetAll()
    {
        var data = await _secretaryRepository.GetAll();
        if (data == null)
            return null;

        List<SecretaryDomainModel> results = new List<SecretaryDomainModel>();
        SecretaryDomainModel secretaryModel;
        foreach (var item in data)
        {
            secretaryModel = new SecretaryDomainModel
            {
                isDeleted = item.isDeleted,
                Id = item.Id,
                BirthDate = item.BirthDate,
                Email = item.Email,
                Name = item.Name,
                Phone = item.Phone,
                Surname = item.Surname
            };
            if (item.Credentials != null) {
                secretaryModel.Credentials = new CredentialsDomainModel {
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
                    secretaryModel.Credentials.UserRole = new UserRoleDomainModel {
                        Id = item.Credentials.UserRole.Id,
                        RoleName = item.Credentials.UserRole.RoleName,
                        isDeleted = item.Credentials.UserRole.isDeleted
                    };
                }
            }
            results.Add(secretaryModel);
        }

        return results;
    } 
}