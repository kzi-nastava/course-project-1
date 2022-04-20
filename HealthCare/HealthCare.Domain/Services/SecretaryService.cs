using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using HealthCare.Repositories;

namespace HealthCare.Domain.Services;

public class SecretaryService : ISecretaryService{
    private ISecretaryRepository _secretaryRepository;

    public SecretaryService(ISecretaryRepository secretaryRepository) {
        _secretaryRepository = secretaryRepository;
    }

    public Task<IEnumerable<SecretaryDomainModel>> GetAll()
    {
        throw new NotImplementedException();
    } 
}