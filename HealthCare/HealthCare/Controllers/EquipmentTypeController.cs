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
            IEnumerable<EquipmentTypeDomainModel> credentials = await _equipmentTypeService.GetAll();
            if (credentials == null) {
                credentials = new List<EquipmentTypeDomainModel>();
            }
            return Ok(credentials);
        }
    }
}
