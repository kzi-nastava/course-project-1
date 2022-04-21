using System.Diagnostics.Eventing.Reader;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareAPI.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class EquipmentController : ControllerBase {
        private IEquipmentService _equipmentService;

        public EquipmentController(IEquipmentService equipmentService) {
            _equipmentService = equipmentService;
        }

        // https://localhost:7195/api/equipment
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EquipmentDomainModel>>> GetAll() {
            IEnumerable<EquipmentDomainModel> equipment = await _equipmentService.GetAll();
            if (equipment == null) {
                equipment = new List<EquipmentDomainModel>();
            }
            return Ok(equipment);
        }
    }
}
