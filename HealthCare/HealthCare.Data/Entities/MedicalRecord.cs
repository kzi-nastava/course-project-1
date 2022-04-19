using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCare.Data.Entities
{
    [Table("medical_record")]
    internal class MedicalRecord
    {
        [Column("height")]
        private double Height { get; set; }

        [Column("weight")]
        private double Weight { get; set; }

        [Column("bedridden_diseases")]
        private string BedriddenDiseases { get; set; }

        [Column("alergies")]
        private string Allergies { get; set; }

        [Column("Patient_id")]
        private int PatientId { get; set; }

        [Column("deleted")]
        private bool isDeleted { get; set; }
    }
}
