using HealthCare.Domain.DTOs;
using HealthCare.Domain.Models;
namespace HealthCare.Domain.Interfaces;

public interface IOperationService : IService<OperationDomainModel> 
{
    public Task<IEnumerable<OperationDomainModel>> GetAllForDoctor(decimal id);
    public Task<OperationDomainModel> Create(CUOperationDTO dto);
    public Task<OperationDomainModel> Update(CUOperationDTO dto);
    public Task<OperationDomainModel> Delete(decimal id);
    public Task<IEnumerable<OperationDomainModel>> ReadAll();

    public Task<OperationDomainModel> CreateUrgent(CreateUrgentOperationDTO dto, IDoctorService doctorService, INotificationService notificationService);

    public Task<IEnumerable<IEnumerable<RescheduleDTO>>> FindFiveAppointments(CreateUrgentOperationDTO dto,
        IDoctorService doctorService, IPatientService patientService);

    public Task<OperationDomainModel> AppointUrgent(List<RescheduleDTO> dto, INotificationService notificationService);
}