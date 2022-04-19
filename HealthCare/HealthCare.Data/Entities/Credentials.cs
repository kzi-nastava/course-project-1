using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCare.Data.Entities
{
    [Table("creds")]
    public class Credentials
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("username")]
        public string Username { get; set; }

        [Column("password")]
        public string Password { get; set; }

        [Column("Doctor_id")]
        public int doctorId { get; set; }

        [Column("Secretary_Id")]
        public int secretaryId { get; set; }

        [Column("Manager_id")] 
        public int managerId { get; set; }
        
        [Column("Patient_id")]
        public int patientId { get; set; }

        [Column("User_role_id")]
        public int userRoleId { get; set; }
    }
}
