using System.Diagnostics.Eventing.Reader;
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

        public OperationController(IOperationService operationService, IDoctorService doctorService, IPatientService patientService) 
        {
            _operationService = operationService;
            _doctorService = doctorService;
            _patientService = patientService;
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
        public async Task<ActionResult<OperationDomainModel>> Create([FromBody] OperationDomainModel operationModel) 
        {
            try
            {
                OperationDomainModel createdOperationModel = await _operationService.Create(operationModel);
                return Ok(createdOperationModel);
            }
            catch (Exception exception)
            {
                return NotFound(exception.Message);
            }

        }

        [HttpPut]
        [Route("update")]
        public async Task<ActionResult<OperationDomainModel>>  Update([FromBody] OperationDomainModel operationModel) 
        {
            try
            {
                OperationDomainModel updatedOperationModel = await _operationService.Update(operationModel);
                return Ok(updatedOperationModel);
            }
            catch (Exception exception)
            {
                return NotFound(exception.Message);
            }
        }

        [HttpPut]
        [Route("delete/operationId={id}")]
        public async Task<ActionResult<OperationDomainModel>> DeleteOperation([FromBody] OperationDomainModel operationModel)
        {
            try
            {
                OperationDomainModel deletedOperationModel = await _operationService.Delete(operationModel);
                return Ok(deletedOperationModel);
            }
            catch (Exception exception)
            {
                return NotFound(exception.Message);
            }
        }
        
        [HttpPut]
        [Route("urgent/{patientId}/{specializationId}/{duration}")]
        public async Task<ActionResult<IEnumerable<OperationDomainModel>>> CreateUrgentOperation(decimal patientId, decimal specializationId, decimal duration)
        {
            List<OperationDomainModel> operationModels =
                (List<OperationDomainModel>) await _operationService.CreateUrgent(patientId, specializationId, duration, _doctorService, _patientService);
            if (operationModels.Count == 0) return Ok();
            return operationModels;
        }
    }
}
