using HealthCare.Data.Entities;
using HealthCare.Domain.Models;
using HealthCare.Domain.Models.ModelsForCreate;

namespace HealthCare.Domain.Interfaces;

public interface IAnamnesisService : IService<AnamnesisDomainModel> {
    public Task<AnamnesisDomainModel> Create(CreateAnamnesisDomainModel createAnamnesisModel);
    public Task<IEnumerable<AnamnesisDomainModel>> ReadAll();
}