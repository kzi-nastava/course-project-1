using System.Diagnostics.Eventing.Reader;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareAPI.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class AnamnsesisController : ControllerBase {
        private IAnamnesisService _anamnesisService;

        public AnamnsesisController(IAnamnesisService anamnesisService) {
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
    }
}
