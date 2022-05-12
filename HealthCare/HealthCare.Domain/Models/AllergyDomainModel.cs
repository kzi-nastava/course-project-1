using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCare.Domain.Models
{
    public class AllergyDomainModel
    {
        public decimal PatientId { get; set; }

        public decimal IngredientId { get; set; }

        public MedicalRecordDomainModel MedicalRecord { get; set; }
    }
}
