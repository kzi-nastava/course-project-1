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
            return Ok(rooms);
        }
        
        [HttpGet]
        [Route("read")]
        public async Task<ActionResult<IEnumerable<RoomDomainModel>>> ReadAll()
        {
            IEnumerable<RoomDomainModel> rooms = await _roomService.ReadAll();
            return Ok(rooms);
        }
        // https://localhost:7195/api/room/create
        [HttpPost]
        [Route("create")]
        public async Task<ActionResult<RoomDomainModel>> CreateRoom([FromBody] RoomDomainModel roomModel)
        {
            RoomDomainModel insertedRoomModel = await _roomService.Create(roomModel);
            return Ok(insertedRoomModel);
        }

        // https://localhost:7195/api/room/update
        [HttpPut]
        [Route("update")]
        public async Task<ActionResult<RoomDomainModel>> UpdateRoom([FromBody] RoomDomainModel roomModel)
        {
            try
            {
                RoomDomainModel updatedRoomModel = await _roomService.Update(roomModel);
                return Ok(updatedRoomModel);
            }
            catch (Exception exception)
            {
                return NotFound(exception.Message);
            }
        }

        // https://localhost:7195/api/room/delete
        [HttpPost]
        [Route("delete/{id}")]
        public async Task<ActionResult<RoomDomainModel>> DeleteRoom(decimal id) 
        {
            try
            {
                RoomDomainModel deletedRoomModel = await _roomService.Delete(id);
                return Ok(deletedRoomModel);
            }
            catch (Exception exception)
            {
                return NotFound(exception.Message);
            }
        }
    }
}
