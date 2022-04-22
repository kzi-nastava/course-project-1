using System.Diagnostics.Eventing.Reader;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using HealthCare.Domain.Models.ModelsForUpdate;
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
            IEnumerable<ExaminationDomainModel> examinations = await _examinationService.GetAll();
            if (examinations == null) {
                examinations = new List<ExaminationDomainModel>();
            }
            return Ok(examinations);
        }

        [HttpGet]
        [Route("patientId={id}")]
        public async Task<ActionResult<IEnumerable<ExaminationDomainModel>>> GetAllExaminationsForPatient(decimal id) {
            IEnumerable<ExaminationDomainModel> examinations = await _examinationService.GetAllForPatient(id);
            if (examinations == null) {
                examinations = new List<ExaminationDomainModel>();
            }
            return Ok(examinations);
        }

        // https://localhost:7195/api/examination/delete
        [HttpPut]
        [Route("delete")]
        public async Task<ActionResult<PatientDomainModel>> DeleteExamination([FromBody] UpdateExaminationDomainModel id) {
            var deletedPatientModel = await _examinationService.Delete(id);
            return Ok(deletedPatientModel);
        }



    }
}
