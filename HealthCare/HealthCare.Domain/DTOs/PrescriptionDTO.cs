using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCare.Domain.DTOs
{
    public class PrescriptionDTO
    {
        public decimal DrugId { get; set; }

        public decimal PatientId { get; set; }

        public decimal DoctorId { get; set; }

        public DateTime TakeAt { get; set; }

        public int PerDay { get; set; }

        public string MealCombination { get; set; }
    }
}
