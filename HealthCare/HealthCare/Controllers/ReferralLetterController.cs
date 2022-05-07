using System.Diagnostics.Eventing.Reader;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReferralLetterController : ControllerBase
    {
        private IReferralLetterService _referralLetterService;

        public ReferralLetterController(IReferralLetterService referralLetterService)
        {
            _referralLetterService = referralLetterService;
        }

        // https://localhost:7195/api/examination
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReferralLetterDomainModel>>> GetAll()
        {
            IEnumerable<ReferralLetterDomainModel> referralLetters = await _referralLetterService.GetAll();
            return Ok(referralLetters);
        }

    }
}
