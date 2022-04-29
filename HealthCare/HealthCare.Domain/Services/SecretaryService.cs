using HealthCare.Data.Entities;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using HealthCare.Repositories;

namespace HealthCare.Domain.Services;

public class SecretaryService : ISecretaryService
{
    private ISecretaryRepository _secretaryRepository;

    public SecretaryService(ISecretaryRepository secretaryRepository) 
    {
        _secretaryRepository = secretaryRepository;
    }
    
    public async Task<IEnumerable<SecretaryDomainModel>> ReadAll()
    {
        IEnumerable<SecretaryDomainModel> secretaries = await GetAll();
        List<SecretaryDomainModel> result = new List<SecretaryDomainModel>();
        foreach (SecretaryDomainModel item in secretaries)
        {
            if (!item.IsDeleted) result.Add(item);
        }
        return result;
    } 

    // Async awaits info from database
    // GetAll is the equivalent of SELECT *
    public async Task<IEnumerable<SecretaryDomainModel>> GetAll()
    {
        IEnumerable<Secretary> data = await _secretaryRepository.GetAll();
        if (data == null)
            throw new DataIsNullException();

        List<SecretaryDomainModel> results = new List<SecretaryDomainModel>();
        SecretaryDomainModel secretaryModel;
        foreach (Secretary item in data)
        {
            secretaryModel = new SecretaryDomainModel
            {
                IsDeleted = item.IsDeleted,
                Id = item.Id,
                BirthDate = item.BirthDate,
                Email = item.Email,
                Name = item.Name,
                Phone = item.Phone,
                Surname = item.Surname
            };
            if (item.Credentials != null) 
            {
                secretaryModel.Credentials = new CredentialsDomainModel 
                {
                    Id = item.Credentials.Id,
                    Username = item.Credentials.Username,
                    Password = item.Credentials.Password,
                    DoctorId = item.Credentials.DoctorId,
                    SecretaryId = item.Credentials.SecretaryId,
                    ManagerId = item.Credentials.ManagerId,
                    PatientId = item.Credentials.PatientId,
                    UserRoleId = item.Credentials.UserRoleId

                };
                if (item.Credentials.UserRole != null) 
                {
                    secretaryModel.Credentials.UserRole = new UserRoleDomainModel 
                    {
                        Id = item.Credentials.UserRole.Id,
                        RoleName = item.Credentials.UserRole.RoleName,
                        IsDeleted = item.Credentials.UserRole.IsDeleted
                    };
                }
            }
            results.Add(secretaryModel);
        }

        return results;
    } 
}