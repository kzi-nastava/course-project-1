using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCare.Data.Entities
{
    [Table("operation")]
    internal class Operation
    {
        [Column("time_held")]
        private DateTime Duration { get; set; }

        [Column("room_id")]
        private int RoomId  { get; set; }   

        [Column("doctor_id")]
        private int DoctorId { get; set; }

        [Column("patient_id")]
        private int PatientId { get; set; }

        [Column("deleted")]
        private bool isDeleted { get; set; }
    }
}
