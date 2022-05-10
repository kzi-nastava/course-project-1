using System.Diagnostics.Eventing.Reader;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using HealthCare.Domain.DTOs;
using HealthCare.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReferralLetterController : ControllerBase
    {
        private IReferralLetterService _referralLetterService;
        private IExaminationService _examinationService;

        public ReferralLetterController(IReferralLetterService referralLetterService, IExaminationService examinationService)
        {
            _referralLetterService = referralLetterService;
            _examinationService= examinationService;
        }

        // https://localhost:7195/api/referralLetter
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReferralLetterDomainModel>>> GetAll()
        {
            IEnumerable<ReferralLetterDomainModel> referralLetters = await _referralLetterService.GetAll();
            return Ok(referralLetters);
        }

        [HttpGet]
        [Route("createAppointment/{referralId}/{time}")]
        public async Task<ActionResult<IEnumerable<ReferralLetterDomainModel>>> CreateAppointment(decimal referralId, DateTime time)
        {
            ReferralLetterDomainModel referralLetter = await _referralLetterService.CreateAppointment(referralId, time, _examinationService);
            return Ok(referralLetter);
        }

        [HttpPost]
        [Route("create")]
        public async Task<ActionResult<ReferralLetterDomainModel>> Create([FromBody] ReferralLetterDTO referralDTO)
        {
            ReferralLetterDomainModel createdReferralModel = await _referralLetterService.Create(referralDTO);
            return Ok(createdReferralModel);
        }
    }
}
