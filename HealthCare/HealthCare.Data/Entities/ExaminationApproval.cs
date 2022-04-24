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
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public decimal Id { get; set; }
        [Column("state")]
        public string State { get; set; }

        [Column("old_room_id")]
        public decimal OldRoomId { get; set; }

        [Column("old_doctor_id")]
        public decimal OldDoctorId { get; set; }

        [Column("old_date")]
        public DateTime OldStartTime { get; set; }

        [Column("old_patient_id")]
        public decimal OldPatientId { get; set; }

        [Column("new_room_id")]
        public decimal NewRoomId { get; set; }

        [Column("new_doctor_id")]
        public decimal NewDoctorId { get; set; }

        [Column("new_date")]
        public DateTime NewStartTime { get; set; }

        [Column("new_patient_id")]
        public decimal NewPatientId { get; set; }

        [Column("deleted")]
        public bool isDeleted { get; set; }


        //public Examination? Examination { get; set; }

    }
}
