using HealthCare.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCare.Domain.Interfaces
{
    public interface IAvailabilityService
    {
        public Task ValidateUserInput(CUExaminationDTO dto, IPatientService patientService);
    }
}
