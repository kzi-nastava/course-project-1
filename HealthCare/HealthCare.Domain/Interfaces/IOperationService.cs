using HealthCare.Domain.DTOs;
using HealthCare.Domain.Models;
namespace HealthCare.Domain.Interfaces;

public interface IOperationService : IService<OperationDomainModel> 
{
    public Task<IEnumerable<OperationDomainModel>> GetAllForDoctor(decimal id);
    public Task<OperationDomainModel> Create(CUOperationDTO dto, IPatientService patientService, IRoomService roomService, IAvailabilityService availabilityService);
    public Task<OperationDomainModel> Update(CUOperationDTO dto, IPatientService patientService, IRoomService roomService, IAvailabilityService availabilityService);
    public Task<OperationDomainModel> Delete(decimal id);
    public Task<IEnumerable<OperationDomainModel>> ReadAll();

}