﻿using HealthCare.Domain.DTOs;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DrugController : Controller
    {
        IDrugService _drugService;

        public DrugController(IDrugService drugService)
        {
            _drugService = drugService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DrugDomainModel>>> GetAll()
        {
            IEnumerable<DrugDomainModel> drugs = await _drugService.GetAll();
            return Ok(drugs);
        }

        [HttpPut]
        public async Task<ActionResult<DrugDomainModel>> Update([FromQuery] decimal id, [FromQuery] string name)
        {
            DrugDomainModel drug = await _drugService.Update(id, name);
            return Ok(drug);
        }
    }
}
