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
            if (medicalRecords == null) {
                medicalRecords = new List<MedicalRecordDomainModel>();
            }
            return Ok(medicalRecords);
        }
    }
}
