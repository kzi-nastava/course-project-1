using System.Diagnostics.Eventing.Reader;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareAPI.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class OperationController : ControllerBase {
        private IOperationService _operationService;

        public OperationController(IOperationService operationService) {
            _operationService = operationService;
        }

        // https://localhost:7195/api/operation
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OperationDomainModel>>> GetAll() {
            IEnumerable<OperationDomainModel> credentials = await _operationService.GetAll();
            if (credentials == null) {
                credentials = new List<OperationDomainModel>();
            }
            return Ok(credentials);
        }
    }
}
