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
    public class OperationController : ControllerBase 
    {
        private IOperationService _operationService;
        private IDoctorService _doctorService;
        private IPatientService _patientService;
        private IRoomService _roomService;
        private IAvailabilityService _availabilityService;

        public OperationController(IOperationService operationService, IDoctorService doctorService, 
                                   IPatientService patientService, IRoomService roomService, IAvailabilityService availabilityService) 
        {
            _operationService = operationService;
            _doctorService = doctorService;
            _patientService = patientService;
            _roomService = roomService;
            _availabilityService = availabilityService;
        }

        // https://localhost:7195/api/operation
        [HttpGet]
        [Route("read")]
        public async Task<ActionResult<IEnumerable<OperationDomainModel>>> ReadAll() 
        {
            IEnumerable<OperationDomainModel> operations = await _operationService.ReadAll();
            return Ok(operations);
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OperationDomainModel>>> GetAll() 
        {
            IEnumerable<OperationDomainModel> operations = await _operationService.GetAll();
            return Ok(operations);
        }

        [HttpGet]
        [Route("doctorId={id}")]
        public async Task<ActionResult<IEnumerable<OperationDomainModel>>> GetAllForDoctor(decimal id)
        {
            try
            {
                IEnumerable<OperationDomainModel> operations = await _operationService.GetAllForDoctor(id);
                return Ok(operations);
            }
            catch (Exception exception)
            {
                return NotFound(exception.Message);
            }
        }

        [HttpPost]
        [Route("create")]
        public async Task<ActionResult<OperationDomainModel>> Create([FromBody] CUOperationDTO dto) 
        {
            try
            {
                OperationDomainModel createdOperationModel = await _operationService.Create(dto, _patientService, _roomService, _availabilityService);
                return Ok(createdOperationModel);
            }
            catch (Exception exception)
            {
                return NotFound(exception.Message);
            }

        }

        [HttpPut]
        [Route("update")]
        public async Task<ActionResult<OperationDomainModel>>  Update([FromBody] CUOperationDTO dto) 
        {
            try
            {
                OperationDomainModel updatedOperationModel = await _operationService.Update(dto, _patientService, _roomService, _availabilityService);
                return Ok(updatedOperationModel);
            }
            catch (Exception exception)
            {
                return NotFound(exception.Message);
            }
        }

        [HttpPut]
        [Route("delete/{id}")]
        public async Task<ActionResult<OperationDomainModel>> DeleteOperation(decimal id)
        {
            try
            {
                OperationDomainModel deletedOperationModel = await _operationService.Delete(id);
                return Ok(deletedOperationModel);
            }
            catch (Exception exception)
            {
                return NotFound(exception.Message);
            }
        }
    }
}
