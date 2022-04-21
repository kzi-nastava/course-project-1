using System.Diagnostics.Eventing.Reader;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareAPI.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class UserRoleController : ControllerBase {
        private IUserRoleService _userRoleService;

        public UserRoleController(IUserRoleService userRoleService) {
            _userRoleService = userRoleService;
        }

        // https://localhost:7195/api/userRole
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserRoleDomainModel>>> GetAll() {
            IEnumerable<UserRoleDomainModel> credentials = await _userRoleService.GetAll();
            if (credentials == null) {
                credentials = new List<UserRoleDomainModel>();
            }
            return Ok(credentials);
        }
    }
}
