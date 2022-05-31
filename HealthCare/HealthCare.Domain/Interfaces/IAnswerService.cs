using HealthCare.Domain.DTOs;
using HealthCare.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCare.Domain.Interfaces
{
    public interface IAnswerService
    {
        Task<IEnumerable<AnswerDomainModel>> GetAll();
        public Task<IEnumerable<AnswerDomainModel>> GetForDoctor(decimal id);
        public Task<IEnumerable<AnswerDomainModel>> GetForHospital();
        public Task<decimal> GetAverageRating(decimal id);
        public HospitalQuestionDTO RateHospital(HospitalQuestionDTO dto);
        public DoctorQuestionDTO RateDoctor(DoctorQuestionDTO dto);

    }
}
