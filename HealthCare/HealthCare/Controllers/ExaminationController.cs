using System.Diagnostics.Eventing.Reader;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareAPI.Controllers 
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExaminationController : ControllerBase 
    {
        private IExaminationService _examinationService;
        private IDoctorService _doctorService;

        public ExaminationController(IExaminationService examinationService, IDoctorService doctorService) 
        {
            _examinationService = examinationService;
            _doctorService = doctorService;
        }

        // https://localhost:7195/api/examination
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExaminationDomainModel>>> GetAll() 
        {
            IEnumerable<ExaminationDomainModel> examinations = await _examinationService.GetAll();
            return Ok(examinations);
        }
        
        [HttpGet]
        [Route("read")]
        public async Task<ActionResult<IEnumerable<ExaminationDomainModel>>> ReadAll() 
        {
            IEnumerable<ExaminationDomainModel> examinations = await _examinationService.ReadAll();
            return Ok(examinations);
        }

        // https://localhost:7195/api/examination/patientId=___
        [HttpGet]
        [Route("patientId={id}")]
        public async Task<ActionResult<IEnumerable<ExaminationDomainModel>>> GetAllForPatient(decimal id) 
        {
            IEnumerable<ExaminationDomainModel> examinations = await _examinationService.GetAllForPatient(id);
            if (examinations == null) {
                examinations = new List<ExaminationDomainModel>();
            }
            return Ok(examinations);
        }

        // https://localhost:7195/api/examination/patientId=___
        [HttpGet]
        [Route("patientId={id}/sortParam={sortParam}")]
        public async Task<ActionResult<IEnumerable<ExaminationDomainModel>>> GetAllForPatientSorted(decimal id, string sortParam)
        {
            IEnumerable<ExaminationDomainModel> examinations = await _examinationService.GetAllForPatientSorted(id, sortParam);
            if (examinations == null)
            {
                examinations = new List<ExaminationDomainModel>();
            }
            return Ok(examinations);
        }

        [HttpGet]
        [Route("doctorId={id}")]
        public async Task<ActionResult<IEnumerable<ExaminationDomainModel>>> GetAllForDoctor(decimal id)
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
        public async Task<ActionResult<ExaminationDomainModel>> DeleteExamination([FromBody] ExaminationDomainModel examinationModel, bool isPatient) 
        {
            try
            {
                ExaminationDomainModel deletedExaminationModel = await _examinationService.Delete(examinationModel, isPatient);
                return Ok(deletedExaminationModel);
            }
            catch (Exception exception)
            {
                return NotFound(exception.Message);
            }
        }

        [HttpPost]
        [Route("create")]
        public async Task<ActionResult<ExaminationDomainModel>> CreateExamination([FromBody] ExaminationDomainModel examinationModel, bool isPatient) 
        {
            try
            {
                ExaminationDomainModel createdExaminationModel = await _examinationService.Create(examinationModel, isPatient);
                return Ok(createdExaminationModel);
            }
            catch(Exception exception)
            {
                return NotFound(exception.Message);
            }
        }

        [HttpPut]
        [Route("update")]
        public async Task<ActionResult<ExaminationDomainModel>> UpdateExamination([FromBody] ExaminationDomainModel examinationModel, bool isPatient) 
        {
            try
            {
                ExaminationDomainModel updatedExaminationModel = await _examinationService.Update(examinationModel, isPatient);
                return Ok(updatedExaminationModel);
            }
            catch (Exception exception)
            {
                return NotFound(exception.Message);
            }

        }
        // https://localhost:7195/api/examination/search
        [HttpGet]
        [Route("search/patientId={id}/substring={substring}")]
        public async Task<ActionResult<IEnumerable<ExaminationDomainModel>>> GetByName(decimal id, string substring)
        {
            try
            {
                IEnumerable<ExaminationDomainModel> examinations = await _examinationService.SearchByAnamnesis(id, substring);
                return Ok(examinations);
            }
            catch (Exception exception)
            {
                return NotFound(exception.Message);
            }
        }

        [HttpPut]
        [Route("urgent/{patientId}/{specializationId}")]
        public async Task<ActionResult<IEnumerable<ExaminationDomainModel>>> CreateUrgentExamination(decimal patientId, decimal specializationId)
        {
            List<ExaminationDomainModel> operationModels =
                (List<ExaminationDomainModel>) await _examinationService.CreateUrgent(patientId, specializationId, _doctorService);
            if (operationModels.Count == 0) return Ok();
            return operationModels;
        }

    }
}
