using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCare.Data.Entities
{
    [Table("operation")]
    public class Operation
    {
        [Column("time_held")]
        public DateTime Duration { get; set; }

        [Column("room_id")]
        public decimal RoomId  { get; set; }   

        [Column("doctor_id")]
        public decimal DoctorId { get; set; }

        [Column("patient_id")]
        public decimal PatientId { get; set; }

        [Column("deleted")]
        public bool isDeleted { get; set; }

        //public Room Room { get; set; }

        //public Patient Patient { get; set; }
        //public Doctor Doctor { get; set; }
    }
}
