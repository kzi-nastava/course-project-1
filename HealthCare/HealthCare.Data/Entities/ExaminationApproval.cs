using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCare.Data.Entities
{
    [Table("examination_approval")]
    internal class ExaminationApproval
    {
        [Column("state")]
        private string State { get; set; }

        [Column("Examination_Room_id")]
        private int RoomId { get; set; }

        [Column("Examination_Doctor_id")]
        private int DoctorId { get; set; }

        [Column("Examination_examination_started")]
        private DateTime StartTime { get; set; }

        [Column("Examination_Patient_id")]
        private int PatientId { get; set; }

        [Column("deleted")]
        private bool isDeleted { get; set; }
    }
}
