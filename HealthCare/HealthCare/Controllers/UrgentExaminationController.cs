using HealthCare.Domain.DTOs;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UrgentExaminationController : Controller
    {
        private IDoctorService _doctorService;
        private IPatientService _patientService;
        public IRoomService _roomService;
        private INotificationService _notificationService;
        private IUrgentExaminationService _urgentExaminationService;

        public UrgentExaminationController(IExaminationService examinationService,
            IDoctorService doctorService,
            IPatientService patientService,
            IRoomService roomService,
            INotificationService notificationService,
            IUrgentExaminationService urgentExaminationService)
        {
            _doctorService = doctorService;
            _patientService = patientService;
            _roomService = roomService;
            _notificationService = notificationService;
            _urgentExaminationService = urgentExaminationService;
        }

        [HttpPut]
        [Route("urgentList")]
        public async Task<ActionResult<IEnumerable<IEnumerable<RescheduleDTO>>>> CreateUrgentExamination(CreateUrgentExaminationDTO dto)
        {
            ExaminationDomainModel examinationModel = await _urgentExaminationService.CreateUrgent(dto, _doctorService, _notificationService, _roomService);
            if (examinationModel != null) return Ok();
            IEnumerable<IEnumerable<RescheduleDTO>> rescheduleItems = await _urgentExaminationService.FindFiveAppointments(dto, _doctorService, _patientService);
            return Ok(rescheduleItems);
        }

        [HttpPut]
        [Route("urgent")]
        public async Task<ActionResult<ExaminationDomainModel>> RescheduleForUrgentExamination(List<RescheduleDTO> dto)
        {
            ExaminationDomainModel urgentExamination = await _urgentExaminationService.AppointUrgent(dto, _notificationService, _roomService);
            return Ok(urgentExamination);
        }
    }
}
