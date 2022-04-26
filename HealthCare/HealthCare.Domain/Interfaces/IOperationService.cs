using HealthCare.Domain.Models;
using HealthCare.Domain.Models.ModelsForCreate;
using HealthCare.Domain.Models.ModelsForDelete;
using HealthCare.Domain.Models.ModelsForUpdate;

namespace HealthCare.Domain.Interfaces;

public interface IOperationService : IService<OperationDomainModel> {
    public Task<IEnumerable<OperationDomainModel>> GetAllForDoctor(decimal id);
    public Task<CreateOperationDomainModel> Create(CreateOperationDomainModel operationModel);
    public Task<UpdateOperationDomainModel> Update(UpdateOperationDomainModel operationModel);
    public Task<DeleteOperationDomainModel> Delete(DeleteOperationDomainModel operationModel);
}