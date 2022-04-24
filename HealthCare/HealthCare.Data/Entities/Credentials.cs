using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace HealthCare.Data.Entities
{
    [Table("creds")]
    public class Credentials
    {
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [Column("username")]
        public string Username { get; set; }

        [Column("password")]
        public string Password { get; set; }

        [Column("Doctor_id")]
        public decimal? doctorId { get; set; }

        [Column("Secretary_Id")]
        public decimal? secretaryId { get; set; }

        [Column("Manager_id")] 
        public decimal? managerId { get; set; }
        
        [Column("Patient_id")]
        public decimal? patientId { get; set; }

        [Column("User_role_id")]
        public decimal userRoleId { get; set; }

        [Column("deleted")]
        public bool isDeleted { get; set; }

        public UserRole UserRole { get; set; }

        //public Manager Manager { get; set; }
        //public Doctor Doctor { get; set; }
        //public Patient Patient { get; set; }
        //public Secretary Secretary { get; set; }
    }
}
