using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCare.Data.Entities
{
    [Table("anamnesis")]
    public class Anamnesis
    {
        [Column("description")]
        public string Description { get; set; }

        [Column("Examination_Doctor_id")]
        public decimal doctorId { get; set; }

        [Column("Examination_Room_id")]
        public decimal roomId { get; set; }

        [Column("Examination_Patient_id")]
        public decimal patientId { get; set; }

        [Column("Examination_examination_started")]
        public DateTime StartTime { get; set; }

        [Column("deleted")]
        public bool isDeleted { get; set; }

        public Examination Examination { get; set; }

    }
}
