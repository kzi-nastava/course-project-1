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
    }
}
