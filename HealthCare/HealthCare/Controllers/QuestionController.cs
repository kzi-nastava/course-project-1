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
    public class QuestionController : ControllerBase
    {
        private IQuestionService _questionService;

        public QuestionController(IQuestionService questionService)
        {
            _questionService = questionService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<QuestionDomainModel>>> GetAll()
        {
            IEnumerable<QuestionDomainModel> questions = await _questionService.GetAll();
            return Ok(questions);
        }

        [HttpGet]
        [Route("/evauluations-count-avg")]
        public async Task<ActionResult<IEnumerable<AverageCountEvaluationDomainModel>>> GetAverageCountEvaluations()
        {
            IEnumerable<AverageCountEvaluationDomainModel> result = await _questionService.GetAverageCountEvaluations();
            return Ok(result);
        }

        [HttpGet]
        [Route("/stats")]
        public async Task<ActionResult<IEnumerable<QuestionDomainModel>>> GetStats()
        {
            IEnumerable<AverageCountEvaluationDomainModel> result = await _questionService.GetStats();
            return Ok(result);
        }
    }
}
