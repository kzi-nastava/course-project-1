using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCare.Data.Entities
{
    [Table("examination")]
    public class Examination
    {
        [Column("doctor_id")]
        public decimal doctorId { get; set; }

        [Column("room_Id")]
        public decimal roomId { get; set; }

        [Column("patient_id")]
        public decimal patientId { get; set; }

        [Column("examination_started")]
        public DateTime StartTime { get; set; }

        [Column("deleted")]
        public bool IsDeleted { get; set; }
        public Anamnesis? Anamnesis { get; set; }
        //public ExaminationApproval? ExaminationApproval { get; set; }
        //public Patient Patient { get; set; }
        //public Doctor Doctor { get; set; }

    }
}
