using HealthCare.Domain.DTOs;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DaysOffRequestController : Controller
    {
        private IDaysOffRequestService _daysOffRequestService;
        public DaysOffRequestController(IDaysOffRequestService daysOffRequestService)
        {
            _daysOffRequestService = daysOffRequestService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DaysOffRequestDomainModel>>> GetAll()
        {
            IEnumerable<DaysOffRequestDomainModel> anamnesis = await _daysOffRequestService.GetAll();
            return Ok(anamnesis);
        }

        [HttpPost]
        [Route("create")]
        public async Task<ActionResult<DaysOffRequestDomainModel>> Create([FromBody] CreateDaysOffRequestDTO daysOffRequest)
        {
            try
            {
                DaysOffRequestDomainModel daysOffRequestModel = await _daysOffRequestService.Create(daysOffRequest);
                return Ok(daysOffRequestModel);
            }
            catch (Exception exception)
            {
                return NotFound(exception.Message);
            }
        }
    }
}
