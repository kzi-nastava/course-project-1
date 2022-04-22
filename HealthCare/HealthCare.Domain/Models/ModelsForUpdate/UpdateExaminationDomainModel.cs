using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCare.Domain.Models.ModelsForUpdate {
    public class UpdateExaminationDomainModel {
        public decimal doctorId { get; set; }

        public decimal roomId { get; set; }

        public decimal patientId { get; set; }

        public DateTime StartTime { get; set; }
    }
}
