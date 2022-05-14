using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using HealthCare.Domain.DTOs;
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
        public async Task<ActionResult<SimpleRenovationDomainModel>> CreateSimple([FromBody] CreateSimpleRenovationDTO dto)
        {
            try
            {
                SimpleRenovationDomainModel renovationModel = await _renovationService.Create(dto);
                return Ok(renovationModel);
            }
            catch (Exception exception)
            {
                return NotFound(exception.Message);
            }

        }

        [HttpPost]
        [Route("create/join")]
        public async Task<ActionResult<JoinRenovationDomainModel>> CreateJoin([FromBody] CreateJoinRenovationDTO dto)
        {
            try
            {
                JoinRenovationDomainModel renovationModel = await _renovationService.Create(dto);
                return Ok(renovationModel);
            }
            catch (Exception exception)
            {
                return NotFound(exception.Message);
            }

        }

        [HttpPost]
        [Route("create/split")]
        public async Task<ActionResult<SplitRenovationDomainModel>> CreateSplit([FromBody] CreateSplitRenovationDTO dto)
        {
            try
            {
                SplitRenovationDomainModel renovationModel = await _renovationService.Create(dto);
                return Ok(renovationModel);
            }
            catch (Exception exception)
            {
                return NotFound(exception.Message);
            }

        }

        [HttpGet]
        [Route("executeComplex")]
        public async Task<ActionResult<bool>> ExecuteComplex()
        {
            try
            {
                await _renovationService.ExecuteComplexRenovations();
                return Ok(true);
            }
            catch (Exception exception)
            {
                return NotFound(exception.Message);
            }

        }
    }
}