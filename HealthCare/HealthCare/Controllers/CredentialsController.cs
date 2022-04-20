using System.Diagnostics.Eventing.Reader;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareAPI.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class CredentialsController : ControllerBase {
        private ICredentialsService _credentialsService;

        public CredentialsController(ICredentialsService credentialsService)
        {
            _credentialsService = credentialsService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CredentialsDomainModel>>> GetAll()
        {
            IEnumerable<CredentialsDomainModel> credentials = await _credentialsService.GetAll();
            if (credentials == null)
            {
                credentials = new List<CredentialsDomainModel>();
            }
            return Ok(credentials);
        }

        [HttpGet]
        [Route("login/{username}/{password}")]
        public async Task<ActionResult<CredentialsDomainModel>> GetLoggedUser(string username, string password)
        {
            CredentialsDomainModel credentials = await _credentialsService.GetCredentialsByUsernameAndPassword(username, password);
            return Ok(credentials);
        }
    }
}
