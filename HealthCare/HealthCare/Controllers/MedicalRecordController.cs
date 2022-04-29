using System.Diagnostics.Eventing.Reader;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareAPI.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class MedicalRecordController : ControllerBase {
        private IMedicalRecordService _medicalRecordService;

        public MedicalRecordController(IMedicalRecordService medicalRecordService) {
            _medicalRecordService = medicalRecordService;
        }

        // https://localhost:7195/api/medicalRecord
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MedicalRecordDomainModel>>> GetAll() {
            IEnumerable<MedicalRecordDomainModel> medicalRecords = await _medicalRecordService.GetAll();
            return Ok(medicalRecords);
        }
        
        [HttpGet]
        [Route("read")]
        public async Task<ActionResult<IEnumerable<MedicalRecordDomainModel>>> ReadAll() {
            IEnumerable<MedicalRecordDomainModel> medicalRecords = await _medicalRecordService.ReadAll();
            return Ok(medicalRecords);
        }

        [HttpGet]
        [Route("patientId={id}")]
        public async Task<ActionResult<MedicalRecordDomainModel>> GetForPatient(decimal id)
        {
            MedicalRecordDomainModel medicalRecordModel = await _medicalRecordService.GetForPatient(id);
            return medicalRecordModel;
        }

        [HttpPut]
        [Route("update")]
        public async Task<ActionResult<MedicalRecordDomainModel>> Update([FromBody] MedicalRecordDomainModel medicalRecordModel)
        {
            MedicalRecordDomainModel updatedRecord = await _medicalRecordService.Update(medicalRecordModel);
            return Ok(updatedRecord);
        }
    }
}
