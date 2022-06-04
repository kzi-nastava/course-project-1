using HealthCare.Domain.DTOs;
using HealthCare.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecommendExaminationController : ControllerBase
    {
        IRecommendExaminationService _recommendExaminationService;
        IDoctorService _doctorService;
        IPatientService _petientService;
        IAvailabilityService _availabilityService;

        public RecommendExaminationController(IRecommendExaminationService recommendExaminationService, IDoctorService doctorService,
                                              IPatientService patientService, IAvailabilityService availabilityService)
        {
            _recommendExaminationService = recommendExaminationService;
            _doctorService = doctorService;
            _petientService = patientService;
            _availabilityService = availabilityService;
        }

        [HttpPost]
        [Route("recommend")]
        public async Task<ActionResult<IEnumerable<CUExaminationDTO>>> RecommendExaminations([FromBody] ParamsForRecommendingFreeExaminationsDTO paramsDTO)
        {
            try
            {
                IEnumerable<CUExaminationDTO> recommendedExaminations = await _recommendExaminationService.GetRecommendedExaminations(paramsDTO, _doctorService, _availabilityService, _petientService);
                return Ok(recommendedExaminations);
            }
            catch (Exception exception)
            {
                return NotFound(exception.Message);
            }
        }

    }
}
