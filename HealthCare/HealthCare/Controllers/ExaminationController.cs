using System.Diagnostics.Eventing.Reader;
using HealthCare.Domain.DTOs;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using HealthCare.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareAPI.Controllers 
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExaminationController : ControllerBase 
    {
        private IExaminationService _examinationService;
        private IDoctorService _doctorService;
        private IPatientService _patientService;
        private INotificationService _notificationService;
        private IRoomService _roomService;
        private IAntiTrollService _antiTrollService;
        private IAvailabilityService _availabilityService;
        private IFilteringExaminationService _filteringExaminationService;

        public ExaminationController(IExaminationService examinationService, 
            IDoctorService doctorService, 
            IPatientService patientService,
            INotificationService notificationService,
            IRoomService roomService,
            IAntiTrollService antiTrollService,
            IAvailabilityService availabilityService,
            IFilteringExaminationService filteringExaminationService) 
        {
            _examinationService = examinationService;
            _doctorService = doctorService;
            _patientService = patientService;
            _notificationService = notificationService;
            _roomService = roomService;
            _antiTrollService = antiTrollService;
            _availabilityService = availabilityService;
            _filteringExaminationService = filteringExaminationService;
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
            try
            {
                IEnumerable<ExaminationDomainModel> examinations = await _filteringExaminationService.GetAllForPatient(id);
                return Ok(examinations);
            }
            catch (Exception exception)
            {
                return NotFound(exception.Message);
            }

        }

        // https://localhost:7195/api/examination/sort/
        [HttpGet]
        [Route("sort")]
        public async Task<ActionResult<IEnumerable<ExaminationDomainModel>>> GetAllForPatientSorted([FromQuery] SortExaminationDTO dto)
        {
            try
            {
                IEnumerable<ExaminationDomainModel> examinations = await _filteringExaminationService.GetAllForPatientSorted(dto, _doctorService);
                return Ok(examinations);
            }
            catch (Exception exception)
            {
                return NotFound(exception.Message);
            }
        }

        [HttpGet]
        [Route("doctorId={id}")]
        public async Task<ActionResult<IEnumerable<ExaminationDomainModel>>> GetAllForDoctor(decimal id)
        {
            try
            {
                IEnumerable<ExaminationDomainModel> examinations = await _filteringExaminationService.GetAllForDoctor(id);
                return Ok(examinations);
            }
            catch (Exception exception)
            {
                return NotFound(exception.Message);
            }
        }

        // https://localhost:7195/api/examination/delete
        [HttpPut]
        [Route("delete")]
        public async Task<ActionResult<ExaminationDomainModel>> DeleteExamination([FromBody] DeleteExaminationDTO dto) 
        {
            try
            {
                ExaminationDomainModel deletedExaminationModel = await _examinationService.Delete(dto, _antiTrollService);
                return Ok(deletedExaminationModel);
            }
            catch (Exception exception)
            {
                return NotFound(exception.Message);
            }
        }

        [HttpPost]
        [Route("create")]
        public async Task<ActionResult<ExaminationDomainModel>> CreateExamination([FromBody] CUExaminationDTO dto) 
        {
            try
            {
                ExaminationDomainModel createdExaminationModel = await _examinationService.Create(dto, _patientService, _roomService, _availabilityService, _antiTrollService);
                return Ok(createdExaminationModel);
            }
            catch(Exception exception)
            {
                return NotFound(exception.Message);
            }
        }

        [HttpPut]
        [Route("update")]
        public async Task<ActionResult<CUExaminationDTO>> UpdateExamination([FromBody] CUExaminationDTO dto) 
        {
            try
            {
                ExaminationDomainModel updatedExaminationModel = await _examinationService.Update(dto, _patientService, _roomService, _availabilityService, _antiTrollService);
                return Ok(updatedExaminationModel);
            }
            catch (Exception exception)
            {
                return NotFound(exception.Message);
            }

        }

        // https://localhost:7195/api/examination/search
        [HttpGet]
        [Route("search/")]
        public async Task<ActionResult<IEnumerable<ExaminationDomainModel>>> GetByName([FromQuery] SearchByNameDTO dto)
        {
            try
            {
                IEnumerable<ExaminationDomainModel> examinations = await _filteringExaminationService.SearchByAnamnesis(dto);
                return Ok(examinations);

            }
            catch (Exception exception)
            {
                return NotFound(exception.Message);
            }
        }

        

    }
}
