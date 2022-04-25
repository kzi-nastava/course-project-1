using System.Diagnostics.Eventing.Reader;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomController : ControllerBase
    {
        private IRoomService _roomService;

        public RoomController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        // https://localhost:7195/api/room
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomDomainModel>>> GetAll()
        {
            IEnumerable<RoomDomainModel> rooms = await _roomService.GetAll();
            if (rooms == null)
            {
                rooms = new List<RoomDomainModel>();
            }
            return Ok(rooms);
        }
        // https://localhost:7195/api/room/create
        [HttpPost]
        [Route("create")]
        public async Task<ActionResult<RoomDomainModel>> CreateRoom([FromBody]RoomDomainModel room)
        {
            var insertedRoom = await _roomService.Add(room);
            return Ok(insertedRoom);
        }

        // https://localhost:7195/api/room/update
        [HttpPut]
        [Route("update/{id}")]
        public async Task<ActionResult<RoomDomainModel>> UpdateRoom(decimal id, RoomDomainModel room)
        {
            var updatedRoom = await _roomService.Update(room, id);
            return Ok(updatedRoom);
        }

        // https://localhost:7195/api/room/delete
        [HttpPost]
        [Route("delete/{id}")]
        public async Task<ActionResult<RoomDomainModel>> DeleteRoom(decimal id) {
            var deletedRoom = await _roomService.Delete(id);
            return Ok(deletedRoom);
        }


    }
}
