using HealthCare.Data.Entities;
using HealthCare.Domain.DTOs;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using HealthCare.Repositories;

namespace HealthCare.Domain.Services;

public class NotificationService : INotificationService
{
    private INotificationRepository _notificationRepository;

    public NotificationService(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<IEnumerable<NotificationDomainModel>> GetAll()
    {
        IEnumerable<Notification> data = await _notificationRepository.GetAll();
        if (data == null)
            throw new DataIsNullException();

        List<NotificationDomainModel> results = new List<NotificationDomainModel>();
        foreach (Notification notification in data)
            results.Add(ParseToModel(notification));

        return results;
    }

    public async Task<IEnumerable<NotificationDomainModel>> ReadAll()
    {
        return await GetAll();
    }


    public static NotificationDomainModel ParseToModel(Notification notification)
    {
        NotificationDomainModel notificationModel = new NotificationDomainModel
        {
            Id = notification.Id,
            Content = notification.Content,
            CredentialsId = notification.CredentialsId,
            IsSeen = notification.IsSeen,
            Title = notification.Title
        };

        return notificationModel;
    }
    
    public static Notification ParseFromModel(NotificationDomainModel notificationModel)
    {
        Notification notification = new Notification
        {
            Id = notificationModel.Id,
            Content = notificationModel.Content,
            CredentialsId = notificationModel.CredentialsId,
            IsSeen = notificationModel.IsSeen,
            Title = notificationModel.Title
        };

        return notification;
    }

    public async Task<NotificationDomainModel> SendToDoctor(decimal doctorId)
    {
        throw new NotImplementedException();
    }

    public async Task<NotificationDomainModel> SendToPatient(decimal patientId)
    {
        throw new NotImplementedException();
    }
}
