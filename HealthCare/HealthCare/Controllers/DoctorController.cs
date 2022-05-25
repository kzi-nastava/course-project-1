using System.Diagnostics.Eventing.Reader;
using HealthCare.Domain.DTOs;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareAPI.Controllers 
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorController : ControllerBase 
    {
        private IDoctorService _doctorService;

        public DoctorController(IDoctorService doctorService) 
        {
            _doctorService = doctorService;
        }

        // https://localhost:7195/api/doctor
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DoctorDomainModel>>> GetAll() 
        {
            IEnumerable<DoctorDomainModel> doctors = await _doctorService.GetAll();
            return Ok(doctors);
        }
        
        [HttpGet]
        [Route("read")]
        public async Task<ActionResult<IEnumerable<DoctorDomainModel>>> ReadAll() 
        {
            IEnumerable<DoctorDomainModel> doctors = await _doctorService.ReadAll();
            return Ok(doctors);
        }
        // FOR TESTING
        // DELETE LATER
        [HttpGet]
        [Route("testSchedule/{doctorId}/{duration?}")]
        public async Task<ActionResult<IEnumerable<KeyValuePair<DateTime, DateTime>>>> GetAvailableSchedule(decimal doctorId, decimal duration=15) 
        {
            IEnumerable<KeyValuePair<DateTime, DateTime>> schedule = await _doctorService.GetAvailableSchedule(doctorId, duration);
            return Ok(schedule);
        }

        [HttpGet]
        [Route("search")]
        public async Task<ActionResult<IEnumerable<DoctorDomainModel>>> SearchDoctors([FromQuery] SearchDoctorsDTO dto)
        {
            IEnumerable<DoctorDomainModel> doctors = await _doctorService.Search(dto);
            return Ok(doctors);
        }
    }
}
