using System.Diagnostics.Eventing.Reader;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareAPI.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class RoomController : ControllerBase {
        private IRoomService _roomService;

        public RoomController(IRoomService roomService) {
            _roomService = roomService;
        }

        // https://localhost:7195/api/room
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomDomainModel>>> GetAll() {
            IEnumerable<RoomDomainModel> rooms = await _roomService.GetAll();
            if (rooms == null) {
                rooms = new List<RoomDomainModel>();
            }
            return Ok(rooms);
        }
    }
}
