using System.Diagnostics.Eventing.Reader;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareAPI.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase {
        private IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService) {
            _inventoryService = inventoryService;
        }

        // https://localhost:7195/api/inventory
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryDomainModel>>> GetAll() {
            IEnumerable<InventoryDomainModel> credentials = await _inventoryService.GetAll();
            if (credentials == null) {
                credentials = new List<InventoryDomainModel>();
            }
            return Ok(credentials);
        }
    }
}
