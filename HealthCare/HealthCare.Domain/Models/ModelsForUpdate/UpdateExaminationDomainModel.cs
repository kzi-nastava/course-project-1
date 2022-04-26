using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCare.Domain.Models.ModelsForUpdate {
    public class UpdateExaminationDomainModel {
        public decimal OldExaminationId { get; set; }

        public decimal NewPatientId { get; set; }

        public DateTime NewStartTime { get; set; }

        public decimal NewDoctorId { get; set; }

        public bool isPatient { get; set; }
    }
}
        