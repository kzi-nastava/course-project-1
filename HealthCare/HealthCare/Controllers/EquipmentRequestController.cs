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

        // https://localhost:7195/api/equipment
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EquipmentRequestDomainModel>>> GetAll()
        {
            IEnumerable<EquipmentRequestDomainModel> equipment = await _equipmentRequestService.GetAll();
            return Ok(equipment);
        }
    }
}
