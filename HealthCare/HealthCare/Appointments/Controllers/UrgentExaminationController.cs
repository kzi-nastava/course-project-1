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
        private IUrgentExaminationService _urgentExaminationService;

        public UrgentExaminationController(IUrgentExaminationService urgentExaminationService)
        {
            _urgentExaminationService = urgentExaminationService;
        }

        [HttpPut]
        [Route("urgentList")]
        public async Task<ActionResult<IEnumerable<IEnumerable<RescheduleDTO>>>> CreateUrgentExamination(CreateUrgentExaminationDTO dto)
        {
            ExaminationDomainModel? examinationModel = await _urgentExaminationService.CreateUrgent(dto);
            if (examinationModel != null) return Ok();
            IEnumerable<IEnumerable<RescheduleDTO>> rescheduleItems = await _urgentExaminationService.FindFiveAppointments(dto);
            return Ok(rescheduleItems);
        }

        [HttpPut]
        [Route("urgent")]
        public async Task<ActionResult<ExaminationDomainModel>> RescheduleForUrgentExamination(List<RescheduleDTO> dto)
        {
            ExaminationDomainModel urgentExamination = await _urgentExaminationService.AppointUrgent(dto);
            return Ok(urgentExamination);
        }
    }
}
