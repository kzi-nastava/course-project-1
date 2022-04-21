using System.Diagnostics.Eventing.Reader;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareAPI.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class TransferController : ControllerBase {
        private ITransferService _transferService;

        public TransferController(ITransferService transferService) {
            _transferService = transferService;
        }

        // https://localhost:7195/api/transfer
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransferDomainModel>>> GetAll() {
            IEnumerable<TransferDomainModel> credentials = await _transferService.GetAll();
            if (credentials == null) {
                credentials = new List<TransferDomainModel>();
            }
            return Ok(credentials);
        }
    }
}
