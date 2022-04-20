using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using HealthCare.Repositories;

namespace HealthCare.Domain.Services;

public class EquipmentService : IEquipmentService{
    private IEquipmentRepository _equipmentRepository;

    public EquipmentService(IEquipmentRepository equipmentRepository) {
        _equipmentRepository = equipmentRepository;
    }

    public Task<IEnumerable<EquipmentDomainModel>> GetAll()
    {
        throw new NotImplementedException();
    }
}