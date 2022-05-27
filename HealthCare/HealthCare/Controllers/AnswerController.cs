using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using HealthCare.Domain.DTOs;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnswerController : ControllerBase
    {
        private IAnswerService _answerService;

        public AnswerController(IAnswerService answerService)
        {
            _answerService = answerService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AnswerDomainModel>>> GetAll()
        {
            IEnumerable<AnswerDomainModel> answers = await _answerService.GetAll();
            return Ok(answers);
        }
    }
}
