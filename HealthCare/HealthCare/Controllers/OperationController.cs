using System.Diagnostics.Eventing.Reader;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareAPI.Controllers 
{
    [ApiController]
    [Route("api/[controller]")]
    public class OperationController : ControllerBase 
    {
        private IOperationService _operationService;

        public OperationController(IOperationService operationService) 
        {
            _operationService = operationService;
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
        public async Task<ActionResult<IEnumerable<OperationDomainModel>>> GetAllOperationsForDoctor(decimal id)
        {
            IEnumerable<OperationDomainModel> operations = await _operationService.GetAllForDoctor(id);
            if (operations == null)
            {
                operations = new List<OperationDomainModel>();
            }
            return Ok(operations);
        }

        [HttpPost]
        [Route("create")]
        public async Task<ActionResult<OperationDomainModel>> Create([FromBody] OperationDomainModel operationModel) 
        {
            OperationDomainModel createdOperationModel = await _operationService.Create(operationModel);
            return Ok(createdOperationModel);
        }

        [HttpPut]
        [Route("update")]
        public async Task<ActionResult<OperationDomainModel>>  Update([FromBody] OperationDomainModel operationModel) 
        {
            OperationDomainModel updatedOperationModel = await _operationService.Update(operationModel);
            return Ok(updatedOperationModel);
        }

        [HttpPut]
        [Route("delete/operationId={id}")]
        public async Task<ActionResult<OperationDomainModel>> DeleteExamination([FromBody] OperationDomainModel operationModel)
        {
            OperationDomainModel deletedOperationModel = await _operationService.Delete(operationModel);
            return Ok(deletedOperationModel);
        }
    }
}
