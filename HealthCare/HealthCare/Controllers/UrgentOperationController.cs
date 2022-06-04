using HealthCare.Domain.DTOs;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareAPI.Controllers
{
    public class UrgentOperationController : Controller
    {
        private IDoctorService _doctorService;
        private IPatientService _patientService;
        public IRoomService _roomService;
        private IUrgentOperationService _urgentOperationService;
        private IOperationService _operationService;
        private INotificationService _notificationService;

        public UrgentOperationController(IDoctorService doctorService,
                                         IPatientService patientService,
                                         IRoomService roomService,
                                         IUrgentOperationService urgentOperationService,
                                         IOperationService operationService,
                                         INotificationService notificationService)
        {
            _doctorService = doctorService;
            _patientService = patientService;
            _roomService = roomService;
            _urgentOperationService = urgentOperationService;
            _operationService = operationService;
            _notificationService = notificationService;
        }

        [HttpPut]
        [Route("urgentList")]
        public async Task<ActionResult<IEnumerable<IEnumerable<RescheduleDTO>>>> CreateUrgentOperation(CreateUrgentOperationDTO dto)
        {
            OperationDomainModel operationModel = await _urgentOperationService.CreateUrgent(dto, _doctorService, _notificationService, _roomService);
            if (operationModel != null) return Ok();
            IEnumerable<IEnumerable<RescheduleDTO>> rescheduleItems = await _urgentOperationService.FindFiveAppointments(dto, _doctorService, _patientService);
            return Ok(rescheduleItems);
        }

        [HttpPut]
        [Route("urgent")]
        public async Task<ActionResult<OperationDomainModel>> RescheduleForUrgentExamination(List<RescheduleDTO> dto)
        {
            OperationDomainModel urgentExamination = await _urgentOperationService.AppointUrgent(dto, _notificationService, _roomService);
            return Ok(urgentExamination);
        }
    }
}
