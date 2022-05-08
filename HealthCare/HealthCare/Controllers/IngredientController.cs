using System.Diagnostics.Eventing.Reader;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IngredientController : ControllerBase
    {
        private IIngredientService _ingridientService;

        public IngredientController(IIngredientService ingridientService)
        {
            _ingridientService = ingridientService;
        }

        // https://localhost:7195/api/ingridient
        [HttpGet]
        public async Task<ActionResult<IEnumerable<IngredientDomainModel>>> GetAll()
        {
            IEnumerable<IngredientDomainModel> ingridients = await _ingridientService.GetAll();
            return Ok(ingridients);
        }

    }
}
