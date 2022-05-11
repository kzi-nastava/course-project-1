using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RenovationController : ControllerBase
    {
        private IRenovationService _renovationService;

        public RenovationController(IRenovationService renovationService)
        {
            _renovationService = renovationService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RenovationDomainModel>>> GetAll()
        {
            IEnumerable<RenovationDomainModel> renovations = await _renovationService.GetAll();
            return Ok(renovations);
        }

        [HttpGet]
        [Route("/join")]
        public async Task<ActionResult<IEnumerable<JoinRenovationDomainModel>>> GetJoin()
        {
            IEnumerable<JoinRenovationDomainModel> renovations = await _renovationService.GetJoin();
            return Ok(renovations);
        }

        [HttpGet]
        [Route("/split")]
        public async Task<ActionResult<IEnumerable<SplitRenovationDomainModel>>> GetSplit()
        {
            IEnumerable<SplitRenovationDomainModel> renovations = await _renovationService.GetSplit();
            return Ok(renovations);
        }

        [HttpGet]
        [Route("/simple")]
        public async Task<ActionResult<IEnumerable<SimpleRenovationDomainModel>>> GetSimple()
        {
            IEnumerable<SimpleRenovationDomainModel> renovations = await _renovationService.GetSimple();
            return Ok(renovations);
        }

        [HttpPost]
        [Route("create/simple")]
        public async Task<ActionResult<SimpleRenovationDomainModel>> CreateSimple([FromBody] SimpleRenovationDomainModel newRenovationModel)
        {
            try
            {
                SimpleRenovationDomainModel renovationModel = await _renovationService.Create(newRenovationModel);
                return Ok(renovationModel);
            }
            catch (Exception exception)
            {
                return NotFound(exception.Message);
            }

        }

        [HttpPost]
        [Route("create/join/{resultRoomName}&{roomTypeId}")]
        public async Task<ActionResult<JoinRenovationDomainModel>> CreateJoin([FromBody] JoinRenovationDomainModel newRenovationModel,
            string resultRoomName, decimal roomTypeId)
        {
            try
            {
                JoinRenovationDomainModel renovationModel = await _renovationService.Create(newRenovationModel, resultRoomName, roomTypeId);
                return Ok(renovationModel);
            }
            catch (Exception exception)
            {
                return NotFound(exception.Message);
            }

        }

        [HttpPost]
        [Route("create/split/{resultRoomName1}&{resultRoomName2}&{roomTypeId1}&{roomTypeId2}")]
        public async Task<ActionResult<SplitRenovationDomainModel>> CreateSplit([FromBody] SplitRenovationDomainModel newRenovationModel,
            string resultRoomName1, string resultRoomName2, decimal roomTypeId1, decimal roomTypeId2)
        {
            try
            {
                SplitRenovationDomainModel renovationModel = await _renovationService.Create(newRenovationModel, resultRoomName1, resultRoomName2, roomTypeId1, roomTypeId2);
                return Ok(renovationModel);
            }
            catch (Exception exception)
            {
                return NotFound(exception.Message);
            }

        }
    }
}