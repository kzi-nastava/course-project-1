using System.Diagnostics.Eventing.Reader;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareAPI.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class EquipmentTypeController : ControllerBase {
        private IEquipmentTypeService _equipmentTypeService;

        public EquipmentTypeController(IEquipmentTypeService equipmentTypeService) {
            _equipmentTypeService = equipmentTypeService;
        }

        // https://localhost:7195/api/equipmentType
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EquipmentTypeDomainModel>>> GetAll() {
            IEnumerable<EquipmentTypeDomainModel> equipmentTypes = await _equipmentTypeService.GetAll();
            if (equipmentTypes == null) {
                equipmentTypes = new List<EquipmentTypeDomainModel>();
            }
            return Ok(equipmentTypes);
        }
    }
}
