using System.Diagnostics.Eventing.Reader;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareAPI.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorController : ControllerBase {
        private IDoctorService _doctorService;

        public DoctorController(IDoctorService doctorService) {
            _doctorService = doctorService;
        }

        // https://localhost:7195/api/doctor
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DoctorDomainModel>>> GetAll() {
            IEnumerable<DoctorDomainModel> credentials = await _doctorService.GetAll();
            if (credentials == null) {
                credentials = new List<DoctorDomainModel>();
            }
            return Ok(credentials);
        }
    }
}
