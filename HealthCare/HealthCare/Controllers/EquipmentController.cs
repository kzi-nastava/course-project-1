﻿using System.Diagnostics.Eventing.Reader;
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

        // https://localhost:7195/api/equipment/search
        [HttpGet]
        [Route("search/{substring}")]
        public async Task<ActionResult<IEnumerable<EquipmentDomainModel>>> GetByName(string substring) {
            IEnumerable<EquipmentDomainModel> equipment = await _equipmentService.SearchByName(substring);
            return Ok(equipment);
        }

        [HttpGet]
        [Route("filter/{equipmentTypeId:decimal?}/{minAmmount:int?}/{MaxAmmount:int?}/{roomTypeId:decimal?}")]
        public async Task<ActionResult<IEnumerable<EquipmentDomainModel>>> GetFilteredEquipment(decimal equipmentTypeId=-1, int minAmmount=-1, int maxAmmount=-1, decimal roomTypeId = -1)
        {
            IEnumerable<EquipmentDomainModel> equipment = await _equipmentService.Filter(equipmentTypeId, minAmmount, maxAmmount, roomTypeId);
            return Ok(equipment);
        }


    }
}
