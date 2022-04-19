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
        public int doctorId { get; set; }

        [Column("room_Id")]
        public int roomId { get; set; }

        [Column("patient_id")]
        public int patientId { get; set; }

        [Column("examination_started")]
        public DateTime StartTime { get; set; }

        [Column("deleted")]
        public bool IsDeleted { get; set; }
    }
}
