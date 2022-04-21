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
            IEnumerable<PatientDomainModel> patients = await _patientService.GetAll();
            if (patients == null) {
                patients = new List<PatientDomainModel>();
            }
            return Ok(patients);
        }
        
        // https://localhost:7195/api/patient/create
        [HttpPost]
        [Route("create")]
        public async Task<ActionResult<PatientDomainModel>> CreatePatient([FromBody] PatientDomainModel patientModel)
        {
            var insertedPatientModel = await _patientService.Add(patientModel);
            return Ok(insertedPatientModel);
        }
        
        // https://localhost:7195/api/patient/update
        [HttpPut]
        [Route("update")]
        public async Task<ActionResult<PatientDomainModel>> UpdatePatient([FromBody] PatientDomainModel patientModel)
        {
            var updatedPatientModel = await _patientService.Update(patientModel);
            return Ok(updatedPatientModel);
        }
        
        // https://localhost:7195/api/patient/delete
        [HttpPut]
        [Route("delete")]
        public async Task<ActionResult<PatientDomainModel>> DeletePatient([FromBody] PatientDomainModel patientModel)
        {
            var deletedPatientModel = await _patientService.Delete(patientModel);
            return Ok(deletedPatientModel);
        }
    }
}
