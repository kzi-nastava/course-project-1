using System.Diagnostics.Eventing.Reader;
using System.Globalization;
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
            IEnumerable<TransferDomainModel> transfers = await _transferService.GetAll();
            return Ok(transfers);
        }
        [HttpGet]
        [Route("read")]
        public async Task<ActionResult<IEnumerable<TransferDomainModel>>> ReadAll() {
            IEnumerable<TransferDomainModel> transfers = await _transferService.ReadAll();
            return Ok(transfers);
        }

        [HttpPost]
        [Route("create")]
        public async Task<ActionResult<TransferDomainModel>> CreateTransfer([FromBody] TransferDomainModel newTransfer)
        {
            newTransfer = await _transferService.Add(newTransfer);
            return Ok(newTransfer);
        }

        // add to Program.cs
        [HttpGet]
        [Route("doTransfer")]
        public async Task<ActionResult<TransferDomainModel>> DoTransfer()
        {
            IEnumerable<TransferDomainModel> transfers = await _transferService.DoTransfers();
            return Ok(transfers);
        }
    }
}
