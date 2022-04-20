using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using HealthCare.Repositories;

namespace HealthCare.Domain.Services;

public class EquipmentTypeService : IEquipmentTypeService {
    private IEquipmentTypeRepository _equipmentTypeRepository;

    public EquipmentTypeService(IEquipmentTypeRepository equipmentTypeRepository)
    {
        _equipmentTypeRepository = equipmentTypeRepository;
    }

    public Task<IEnumerable<EquipmentTypeDomainModel>> GetAll()
    {
        throw new NotImplementedException();
    }
}