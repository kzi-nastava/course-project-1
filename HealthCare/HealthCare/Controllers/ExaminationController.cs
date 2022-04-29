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
            IEnumerable<ExaminationDomainModel> examinations = await _examinationService.GetAll();
            return Ok(examinations);
        }
        
        [HttpGet]
        [Route("read")]
        public async Task<ActionResult<IEnumerable<ExaminationDomainModel>>> ReadAll() {
            IEnumerable<ExaminationDomainModel> examinations = await _examinationService.ReadAll();
            return Ok(examinations);
        }

        // https://localhost:7195/api/examination/patientId=___
        [HttpGet]
        [Route("patientId={id}")]
        public async Task<ActionResult<IEnumerable<ExaminationDomainModel>>> GetAllExaminationsForPatient(decimal id) {
            IEnumerable<ExaminationDomainModel> examinations = await _examinationService.GetAllForPatient(id);
            if (examinations == null) {
                examinations = new List<ExaminationDomainModel>();
            }
            return Ok(examinations);
        }

        [HttpGet]
        [Route("doctorId={id}")]
        public async Task<ActionResult<IEnumerable<ExaminationDomainModel>>> GetAllExaminationsForDoctor(decimal id)
        {
            IEnumerable<ExaminationDomainModel> examinations = await _examinationService.GetAllForDoctor(id);
            if (examinations == null)
            {
                examinations = new List<ExaminationDomainModel>();
            }
            return Ok(examinations);
        }

        // https://localhost:7195/api/examination/delete
        [HttpPut]
        [Route("delete")]
        public async Task<ActionResult<ExaminationDomainModel>> DeleteExamination([FromBody] ExaminationDomainModel examinationModel, bool isPatient) {
            var deletedExaminationModel = await _examinationService.Delete(examinationModel, true);
            return Ok(deletedExaminationModel);
        }

        [HttpPost]
        [Route("create")]
        public async Task<ActionResult<ExaminationDomainModel>> CreateExamination([FromBody] ExaminationDomainModel examinationModel, bool isPatient) {
            var createdExaminationModel = await _examinationService.Create(examinationModel, isPatient);
            return Ok(createdExaminationModel);
        }

        [HttpPut]
        [Route("update")]
        public async Task<ActionResult<ExaminationDomainModel>> UpdateExamination([FromBody] ExaminationDomainModel examinationModel, bool isPatient) {
            var updatedExaminationModel = await _examinationService.Update(examinationModel, isPatient);
            return Ok(updatedExaminationModel);
        }




    }
}
