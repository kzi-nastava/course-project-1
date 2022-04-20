using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCare.Data.Entities
{
    [Table("examination_approval")]
    public class ExaminationApproval
    {
        [Column("state")]
        public string State { get; set; }

        [Column("Examination_Room_id")]
        public decimal RoomId { get; set; }

        [Column("Examination_Doctor_id")]
        public decimal DoctorId { get; set; }

        [Column("Examination_examination_started")]
        public DateTime StartTime { get; set; }

        [Column("Examination_Patient_id")]
        public decimal PatientId { get; set; }

        [Column("deleted")]
        public bool isDeleted { get; set; }

        public Examination Examination { get; set; }

    }
}
