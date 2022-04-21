﻿using System.Diagnostics.Eventing.Reader;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace HealthCareAPI.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class ExaminationApprovalController : ControllerBase {
        private IExaminationApprovalService _examinationApprovalService;

        public ExaminationApprovalController(IExaminationApprovalService examinationApprovalService) {
            _examinationApprovalService = examinationApprovalService;
        }

        // https://localhost:7195/api/examinationApproval
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExaminationApprovalDomainModel>>> GetAll() {
            IEnumerable<ExaminationApprovalDomainModel> credentials = await _examinationApprovalService.GetAll();
            if (credentials == null) {
                credentials = new List<ExaminationApprovalDomainModel>();
            }
            return Ok(credentials);
        }
    }
}