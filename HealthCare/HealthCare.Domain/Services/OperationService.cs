using HealthCare.Domain.Models;
using HealthCare.Repositories;

namespace HealthCare.Domain.Interfaces;

public class OperationService : IOperationService{
    private IOperationRepository _operationRepository;

    public OperationService(IOperationRepository operationRepository) {
        _operationRepository = operationRepository;
    }

    public Task<IEnumerable<OperationDomainModel>> GetAll()
    {
        throw new NotImplementedException();
    } 
}