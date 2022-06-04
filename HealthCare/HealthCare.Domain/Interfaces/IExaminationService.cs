using HealthCare.Domain.DTOs;
using HealthCare.Domain.Models;

namespace HealthCare.Domain.Interfaces;

public interface IExaminationService : IService<ExaminationDomainModel> 
{
    public Task<ExaminationDomainModel> Delete(DeleteExaminationDTO dto, IAntiTrollService antiTrollService);
    public Task<ExaminationDomainModel> Create(CUExaminationDTO dto, IPatientService patientService, IRoomService roomService, IAvailabilityService availabilityService, IAntiTrollService antiTrollService);
    public Task<ExaminationDomainModel> Update(CUExaminationDTO dto, IPatientService patientService, IRoomService roomService, IAvailabilityService availabilityService, IAntiTrollService antiTrollService);
    public Task<IEnumerable<ExaminationDomainModel>> ReadAll();

}