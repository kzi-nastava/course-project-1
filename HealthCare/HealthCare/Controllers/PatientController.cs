using System.Diagnostics.Eventing.Reader;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareAPI.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class PatientController : ControllerBase {
        private IPatientService _patientService;

        public PatientController(IPatientService patientService) {
            _patientService = patientService;
        }

        // https://localhost:7195/api/patient
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PatientDomainModel>>> GetAll() {
            IEnumerable<PatientDomainModel> credentials = await _patientService.GetAll();
            if (credentials == null) {
                credentials = new List<PatientDomainModel>();
            }
            return Ok(credentials);
        }
    }
}
