using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCare.Domain.Models.ModelsForUpdate {
    public class UpdatePatientDomainModel {
        public string Name { get; set; }

        public string Surname { get; set; }

        public string Email { get; set; }

        public DateTime BirthDate { get; set; }

        public string Phone { get; set; }

        public string? blockedBy { get; set; }

        public decimal blockingCounter { get; set; }

        public bool IsDeleted { get; set; }

        public UpdateMedicalRecordDomainModel MedicalRecord { get; set; }
        public UpdateCredentialsPatientDomainModel Credentials { get; set; }
    }
}
