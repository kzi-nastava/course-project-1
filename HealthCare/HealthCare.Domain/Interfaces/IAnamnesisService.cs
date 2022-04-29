using HealthCare.Data.Entities;
using HealthCare.Domain.Models;

namespace HealthCare.Domain.Interfaces;

public interface IAnamnesisService : IService<AnamnesisDomainModel> {
    public Task<AnamnesisDomainModel> Create(AnamnesisDomainModel anamnesisModel);
    public Task<IEnumerable<AnamnesisDomainModel>> ReadAll();
}