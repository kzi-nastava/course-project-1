using System.Diagnostics.Eventing.Reader;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareAPI.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class ExaminationController : ControllerBase {
        private IExaminationService _examinationService;

        public ExaminationController(IExaminationService examinationService) {
            _examinationService = examinationService;
        }

        // https://localhost:7195/api/examination
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExaminationDomainModel>>> GetAll() {
            IEnumerable<ExaminationDomainModel> credentials = await _examinationService.GetAll();
            if (credentials == null) {
                credentials = new List<ExaminationDomainModel>();
            }
            return Ok(credentials);
        }
    }
}
