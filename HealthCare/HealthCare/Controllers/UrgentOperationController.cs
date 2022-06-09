using HealthCare.Domain.DTOs;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UrgentOperationController : Controller
    {
        private IUrgentOperationService _urgentOperationService;

        public UrgentOperationController(IUrgentOperationService urgentOperationService)
        {
            _urgentOperationService = urgentOperationService;
        }

        [HttpPut]
        [Route("urgentList")]
        public async Task<ActionResult<IEnumerable<IEnumerable<RescheduleDTO>>>> CreateUrgentOperation(CreateUrgentOperationDTO dto)
        {
            OperationDomainModel operationModel = await _urgentOperationService.CreateUrgent(dto);
            if (operationModel != null) return Ok();
            IEnumerable<IEnumerable<RescheduleDTO>> rescheduleItems = await _urgentOperationService.FindFiveAppointments(dto);
            return Ok(rescheduleItems);
        }

        [HttpPut]
        [Route("urgent")]
        public async Task<ActionResult<OperationDomainModel>> RescheduleForUrgentOperation(List<RescheduleDTO> dto)
        {
            OperationDomainModel urgentOperation = await _urgentOperationService.AppointUrgent(dto);
            return Ok(urgentOperation);
        }
    }
}
