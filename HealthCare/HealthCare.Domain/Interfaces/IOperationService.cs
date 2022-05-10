using HealthCare.Domain.Models;
namespace HealthCare.Domain.Interfaces;

public interface IOperationService : IService<OperationDomainModel> 
{
    public Task<IEnumerable<OperationDomainModel>> GetAllForDoctor(decimal id);
    public Task<OperationDomainModel> Create(OperationDomainModel operationModel);
    public Task<OperationDomainModel> Update(OperationDomainModel operationModel);
    public Task<OperationDomainModel> Delete(OperationDomainModel operationModel);
    public Task<IEnumerable<OperationDomainModel>> ReadAll();

    public Task<IEnumerable<OperationDomainModel>> CreateUrgent(decimal patientId, decimal specializationId,
        decimal duration, IDoctorService doctorService, IPatientService patientService);
}