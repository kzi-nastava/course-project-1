using System.Data;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using HealthCare.Repositories;

namespace HealthCare.Domain.Services;

public class AnamnesisService : IAnamnesisService
{
    private IAnamnesisRepository _anamnesisRepository;

    public AnamnesisService(IAnamnesisRepository anamnesisRepository) {
        _anamnesisRepository = anamnesisRepository;
    }

    public Task<IEnumerable<AnamnesisDomainModel>> GetAll()
    {
        throw new NotImplementedException();
    }
}