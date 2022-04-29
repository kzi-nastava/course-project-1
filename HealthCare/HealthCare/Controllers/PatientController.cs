using System.Diagnostics.Eventing.Reader;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareAPI.Controllers 
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientController : ControllerBase 
    {
        private IPatientService _patientService;

        public PatientController(IPatientService patientService) 
        {
            _patientService = patientService;
        }

        // https://localhost:7195/api/patient
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PatientDomainModel>>> GetAll() 
        {
            IEnumerable<PatientDomainModel> patients = await _patientService.GetAll();
            return Ok(patients);
        }
        
        [HttpGet]
        [Route("read")]
        public async Task<ActionResult<IEnumerable<PatientDomainModel>>> ReadAll() 
        {
            IEnumerable<PatientDomainModel> patients = await _patientService.ReadAll();
            return Ok(patients);
        }

        [HttpGet]
        [Route("medical_record/patientId={id}")]
        public async Task<ActionResult<PatientDomainModel>> GetWithMedicalRecord(decimal id)
        {
            try
            {
                PatientDomainModel patientModel = await _patientService.GetWithMedicalRecord(id);
                return Ok(patientModel);
            }
            catch (Exception exception)
            {
                return NotFound(exception.Message);
            }
        }

        // https://localhost:7195/api/patient/create
        [HttpPost]
        [Route("create")]
        public async Task<ActionResult<PatientDomainModel>> CreatePatient([FromBody] PatientDomainModel patientModel)
        {
            PatientDomainModel insertedPatientModel = await _patientService.Add(patientModel);
            return Ok(insertedPatientModel);
        }
        
        // https://localhost:7195/api/patient/update
        [HttpPut]
        [Route("update/{id}")]
        public async Task<ActionResult<PatientDomainModel>> UpdatePatient([FromBody] PatientDomainModel patientModel)
        {
            try
            {
                PatientDomainModel updatedPatientModel = await _patientService.Update(patientModel);
                return Ok(updatedPatientModel);
            }
            catch (Exception exception)
            {
                return NotFound(exception.Message);
            }
        }
        
        // https://localhost:7195/api/patient/delete
        [HttpPut]
        [Route("delete/{id}")]
        public async Task<ActionResult<PatientDomainModel>> DeletePatient([FromBody] PatientDomainModel patientModel)
        {
            try
            {
                PatientDomainModel deletedPatientModel = await _patientService.Delete(patientModel);
                return Ok(deletedPatientModel);
            }
            catch (Exception exception)
            {
                return NotFound(exception.Message);
            }
        }
        
        // https://localhost:7195/api/patient/block
        [HttpPut]
        [Route("block/{id}")]
        public async Task<ActionResult<PatientDomainModel>> BlockPatient(decimal id)
        {
            PatientDomainModel blockedPatient = await _patientService.Block(id);
            return Ok(blockedPatient);
        }
        
        [HttpGet]
        [Route("block")]
        public async Task<ActionResult<IEnumerable<PatientDomainModel>>> GetBlockedPatients()
        {
            IEnumerable<PatientDomainModel> blockedPatients = await _patientService.GetBlockedPatients();
            return Ok(blockedPatients);
        }
        
        // https://localhost:7195/api/patient/unblock
        [HttpPut]
        [Route("unblock/{id}")]
        public async Task<ActionResult<PatientDomainModel>> UnblockPatient(decimal id)
        {
            PatientDomainModel blockedPatient = await _patientService.Unblock(id);
            return Ok(blockedPatient);
        }
    }
}
