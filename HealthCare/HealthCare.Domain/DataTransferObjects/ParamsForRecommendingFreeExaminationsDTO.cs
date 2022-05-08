using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCare.Domain.DataTransferObjects
{
    public class ParamsForRecommendingFreeExaminationsDTO
    {
        public decimal PatientId { get; set; }
        public decimal DoctorId { get; set; }
        public TimeOnly TimeFrom { get; set; }
        public TimeOnly TimeTo { get; set; }
        public DateOnly LastDate { get; set; }
        public bool IsDoctorPriority { get; set; }

    }
}