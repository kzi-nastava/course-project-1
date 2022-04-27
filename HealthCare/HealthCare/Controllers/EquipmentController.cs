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
            return Ok(equipment);
        }
        
        [HttpGet]
        [Route("read")]
        public async Task<ActionResult<IEnumerable<EquipmentDomainModel>>> ReadAll() {
            IEnumerable<EquipmentDomainModel> equipment = await _equipmentService.ReadAll();
            return Ok(equipment);
        }
        

        // https://localhost:7195/api/equipment/search
        [HttpGet]
        [Route("search/{nameAlike}")]
        public async Task<ActionResult<IEnumerable<EquipmentDomainModel>>> GetByName(string nameAlike) {
            IEnumerable<EquipmentDomainModel> equipment = await _equipmentService.SearchByName(nameAlike);
            return Ok(equipment);
        }

        [HttpGet]
        [Route("filter/{equipmentTypeId:decimal?}/{minAmmount:int?}/{MaxAmmount:int?}/{roomTypeId:decimal?}")]
        public async Task<ActionResult<IEnumerable<EquipmentDomainModel>>> GetFilteredEquipment(decimal equipmentTypeId=-1, int minAmmount=-1, int MaxAmmount=-1, decimal roomTypeId = -1)
        {
            IEnumerable<EquipmentDomainModel> equipment = await _equipmentService.Filter(equipmentTypeId, minAmmount, MaxAmmount, roomTypeId);
            return Ok(equipment);
        }

        [HttpPost]
        [Route("transfer/{roomIdIn:decimal}&{roomIdOut:decimal}&{equipmentId}&{amount:decimal}")]
        public async Task<ActionResult<IEnumerable<EquipmentDomainModel>>> TransferEquipment(decimal roomIdIn, decimal roomIdOut, decimal equipmentID, decimal amount)
        {
            Tuple<EquipmentDomainModel, EquipmentDomainModel> equipment = await _equipmentService.Transfer(roomIdIn, roomIdOut, equipmentID, amount);
            return Ok(equipment);
        }

    }
}
