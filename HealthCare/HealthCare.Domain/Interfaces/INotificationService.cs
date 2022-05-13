using HealthCare.Domain.Models;

namespace HealthCare.Domain.Interfaces;

public interface INotificationService : IService<NotificationDomainModel>
{
    public Task<NotificationDomainModel> SendToDoctor(decimal doctorId);
    public Task<NotificationDomainModel> SendToPatient(decimal patientId);
}
