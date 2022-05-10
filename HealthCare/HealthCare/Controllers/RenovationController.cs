using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RenovationController : ControllerBase
    {
        private IRenovationService _renovationService;

        public RenovationController(IRenovationService renovationService)
        {
            _renovationService = renovationService;
        }

        // https://localhost:7195/api/anamnsesis
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AnamnesisDomainModel>>> GetAll()
        {
            IEnumerable<RenovationDomainModel> anamnesis = await _renovationService.GetAll();
            return Ok(anamnesis);
        }

        [HttpPost]
        [Route("create")]
        public async Task<ActionResult<RenovationDomainModel>> Create([FromBody] RenovationDomainModel newRenovationModel)
        {
            try
            {
                RenovationDomainModel renovationModel = await _renovationService.Create(newRenovationModel);
                return Ok(renovationModel);
            }
            catch (Exception exception)
            {
                return NotFound(exception.Message);
            }

        }
    }
}