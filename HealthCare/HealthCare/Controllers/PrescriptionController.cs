using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrescriptionController : ControllerBase
    {
        IPrescriptionService _prescriptionService;

        public PrescriptionController(IPrescriptionService prescriptionService)
        {
            _prescriptionService = prescriptionService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PrescriptionDomainModel>>> GetAll()
        {
            IEnumerable<PrescriptionDomainModel> prescriptions = await _prescriptionService.GetAll();
            return Ok(prescriptions);
        }

        [HttpPost]
        [Route("create")]
        public async Task<ActionResult<PrescriptionDomainModel>> Create([FromBody] PrescriptionDomainModel newPrescriptionModel)
        {
            try
            {
                PrescriptionDomainModel prescriptionModel = await _prescriptionService.Create(newPrescriptionModel);
                return Ok(prescriptionModel);
            }
            catch (Exception exception)
            {
                return NotFound(exception.Message);
            }
        }

    }
}
