using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCare.Domain.Models.ModelsForCreate {
    public class CreateMedicalRecordDomainModel {
        public decimal Height { get; set; }

        public decimal Weight { get; set; }

        public string BedriddenDiseases { get; set; }

        public string Allergies { get; set; }
    }
}
