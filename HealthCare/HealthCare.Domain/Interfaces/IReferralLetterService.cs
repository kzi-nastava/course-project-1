using HealthCare.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthCare.Domain.Services;

namespace HealthCare.Domain.Interfaces
{
    public interface IReferralLetterService : IService<ReferralLetterDomainModel>
    {
        public Task<ReferralLetterDomainModel> CreateAppointment(decimal referralId, DateTime time, IExaminationService examinationService);
    }
}
