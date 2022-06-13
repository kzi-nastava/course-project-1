using HealthCare.Domain.DTOs;
using HealthCare.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCare.Domain.Interfaces
{
    public interface IUrgentExaminationService
    {
        public Task<ExaminationDomainModel> CreateUrgent(CreateUrgentExaminationDTO dto);

        public Task<IEnumerable<IEnumerable<RescheduleDTO>>> FindFiveAppointments(CreateUrgentExaminationDTO dto);

        public Task<ExaminationDomainModel> AppointUrgent(List<RescheduleDTO> dto);
    }
}
