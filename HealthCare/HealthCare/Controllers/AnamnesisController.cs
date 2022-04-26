using System.Diagnostics.Eventing.Reader;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using HealthCare.Domain.Models.ModelsForCreate;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareAPI.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class AnamnesisController : ControllerBase {
        private IAnamnesisService _anamnesisService;

        public AnamnesisController(IAnamnesisService anamnesisService) {
            _anamnesisService = anamnesisService;
        }

        // https://localhost:7195/api/anamnsesis
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AnamnesisDomainModel>>> GetAll() {
            IEnumerable<AnamnesisDomainModel> anamnesis =  await _anamnesisService.GetAll();
            if (anamnesis == null) {
                anamnesis = new List<AnamnesisDomainModel>();
            }
            return Ok(anamnesis);
        }

        [HttpPost]
        [Route("create")]
        public async Task<ActionResult<AnamnesisDomainModel>> Create([FromBody] AnamnesisDomainModel model)
        {
            AnamnesisDomainModel domainModel = await _anamnesisService.Create(model);
            return Ok(domainModel);
        }
    }
}
