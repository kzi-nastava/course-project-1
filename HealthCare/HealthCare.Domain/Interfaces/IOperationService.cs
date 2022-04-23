using HealthCare.Domain.Models;
using HealthCare.Domain.Models.ModelsForCreate;
using HealthCare.Domain.Models.ModelsForDelete;
using HealthCare.Domain.Models.ModelsForUpdate;

namespace HealthCare.Domain.Interfaces;

public interface IOperationService : IService<OperationDomainModel> {
    public Task<IEnumerable<OperationDomainModel>> GetAllForDoctor(decimal id);
    public Task<IEnumerable<OperationDomainModel>> Create(CreateOperationDomainModel operationModel);
    public Task<IEnumerable<OperationDomainModel>> Update(UpdateOperationDomainModel operationModel);
    public Task<IEnumerable<OperationDomainModel>> Delete(DeleteOperationDomainModel operationModel);
}