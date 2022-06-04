using HealthCare.Domain.DTOs;
using HealthCare.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCare.Domain.Interfaces
{
    public interface IUrgentOperationService
    {
        public Task<OperationDomainModel> CreateUrgent(CreateUrgentOperationDTO dto, IDoctorService doctorService, INotificationService notificationService, IRoomService roomService);
        public Task<IEnumerable<IEnumerable<RescheduleDTO>>> FindFiveAppointments(CreateUrgentOperationDTO dto,
            IDoctorService doctorService, IPatientService patientService);
        public Task<OperationDomainModel> AppointUrgent(List<RescheduleDTO> dto, INotificationService notificationService, IRoomService roomService);
    }
}
