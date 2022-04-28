using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCare.Domain.Models.ModelsForUpdate {
    public class DeleteExaminationDomainModel {

        public decimal ExaminationId { get; set; }

        public decimal PatientId { get; set; }

        public bool IsPatient { get; set; }
    }
}
