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
                Credentials = item.Credentials,
                Email = item.Email,
                Name = item.Name,
                Phone = item.Phone,
                Surname = item.Surname
            };
            results.Add(secretaryModel);
        }

        return results;
    } 
}