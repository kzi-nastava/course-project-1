using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCare.Domain.Models.ModelsForCreate {
    public class CreateExaminationDomainModel {
        public decimal doctorId { get; set; }

        public decimal patientId { get; set; }

        public DateTime StartTime { get; set; }

    }
}
