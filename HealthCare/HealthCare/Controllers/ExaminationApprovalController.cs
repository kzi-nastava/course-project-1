using System.Diagnostics.Eventing.Reader;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareAPI.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class ExaminationApprovalController : ControllerBase {
        private IExaminationApprovalService _examinationApprovalService;

        public ExaminationApprovalController(IExaminationApprovalService examinationApprovalService) {
            _examinationApprovalService = examinationApprovalService;
        }

        // https://localhost:7195/api/examinationApproval
        [HttpGet]
        [Route("read")]
        public async Task<ActionResult<IEnumerable<ExaminationApprovalDomainModel>>> ReadAll() {
            IEnumerable<ExaminationApprovalDomainModel> examinationApprovals = await _examinationApprovalService.ReadAll();
            return Ok(examinationApprovals);
        }
        [HttpGet]
        
        public async Task<ActionResult<IEnumerable<ExaminationApprovalDomainModel>>> GetAll() {
            IEnumerable<ExaminationApprovalDomainModel> examinationApprovals = await _examinationApprovalService.GetAll();
            return Ok(examinationApprovals);
        }
        
        // https://localhost:7195/api/examinationApproval/reject
        [HttpPut]
        [Route("reject")]
        public async Task<ActionResult<IEnumerable<ExaminationApprovalDomainModel>>> Reject([FromBody] ExaminationApprovalDomainModel examinationModel) {
            ExaminationApprovalDomainModel examinationApproval = await _examinationApprovalService.Reject(examinationModel);
            return Ok(examinationApproval);
        }
        
        // https://localhost:7195/api/examinationApproval/approve
        [HttpPut]
        [Route("approve")]
        public async Task<ActionResult<IEnumerable<ExaminationApprovalDomainModel>>> Approve([FromBody] ExaminationApprovalDomainModel examinationModel) {
            ExaminationApprovalDomainModel examinationApproval = await _examinationApprovalService.Approve(examinationModel);
            return Ok(examinationApproval);
        }
    }
}
