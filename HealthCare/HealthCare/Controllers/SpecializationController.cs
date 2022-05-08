using System.Diagnostics.Eventing.Reader;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SpecializationController : ControllerBase
    {
        private ISpecializationService _specializationService;

        public SpecializationController(ISpecializationService specializationService)
        {
            _specializationService = specializationService;
        }

        // https://localhost:7195/api/ingridient
        [HttpGet]
        public async Task<ActionResult<IEnumerable<IngredientDomainModel>>> GetAll()
        {
            IEnumerable<SpecializationDomainModel> ingridients = await _specializationService.GetAll();
            return Ok(ingridients);
        }

    }
}
