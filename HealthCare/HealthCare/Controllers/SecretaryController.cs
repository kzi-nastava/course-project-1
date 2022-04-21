using System.Diagnostics.Eventing.Reader;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareAPI.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class SecretaryController : ControllerBase {
        private ISecretaryService _secretaryService;

        public SecretaryController(ISecretaryService secretaryService) {
            _secretaryService = secretaryService;
        }

        // https://localhost:7195/api/secretary
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SecretaryDomainModel>>> GetAll() {
            IEnumerable<SecretaryDomainModel> credentials = await _secretaryService.GetAll();
            if (credentials == null) {
                credentials = new List<SecretaryDomainModel>();
            }
            return Ok(credentials);
        }
    }
}
