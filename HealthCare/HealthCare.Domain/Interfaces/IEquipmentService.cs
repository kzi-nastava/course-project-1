using HealthCare.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace HealthCare.Domain.Interfaces;

public interface IEquipmentService : IService<EquipmentDomainModel>
{
    public Task<IEnumerable<EquipmentDomainModel>> SearchByName(string nameAlike);
    public Task<IEnumerable<EquipmentDomainModel>> Filter(decimal equipmentTypeId, int minAmount, int maxAmount, decimal roomTypeId);
    public Task<IEnumerable<EquipmentDomainModel>> ReadAll();
}