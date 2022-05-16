using HealthCare.Data.Entities;
using HealthCare.Domain.DTOs;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareAPI.Controllers 
{
    [ApiController]
    [Route("api/[controller]")]
    public class EquipmentRequestController : ControllerBase
    {
        private IEquipmentRequestService _equipmentRequestService;

        public EquipmentRequestController(IEquipmentRequestService equipmentRequestService)
        {
            _equipmentRequestService = equipmentRequestService;
        }

        // https://localhost:7195/api/equipmentRequests
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EquipmentRequestDomainModel>>> GetAll()
        {
            IEnumerable<EquipmentRequestDomainModel> equipment = await _equipmentRequestService.GetAll();
            return Ok(equipment);
        }
        
        [HttpGet]
        [Route("missingEquipment")]
        public async Task<ActionResult<IEnumerable<EquipmentDomainModel>>> GetMissingEquipment()
        {
            IEnumerable<EquipmentDomainModel> equipment = await _equipmentRequestService.GetMissingEquipment();
            return Ok(equipment);
        }
        
        [HttpPut]
        [Route("orderEquipment")]
        public async Task<ActionResult<IEnumerable<EquipmentRequestDomainModel>>> OrderEquipment([FromBody] IEnumerable<EquipmentRequestDTO> dtos)
        {
            IEnumerable<EquipmentRequestDomainModel> equipmentRequests = 
                await _equipmentRequestService.OrderEquipment(dtos);
            return Ok(equipmentRequests);
        }
        
        [HttpPut]
        [Route("doAllOrders")]
        public async Task<ActionResult<IEnumerable<EquipmentRequestDomainModel>>> DoAllOrders()
        {
            IEnumerable<EquipmentRequestDomainModel> equipmentRequests = await _equipmentRequestService.DoAllOrders();
            return Ok(equipmentRequests);
        }
    }
}
