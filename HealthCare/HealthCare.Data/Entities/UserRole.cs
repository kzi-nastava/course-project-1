using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCare.Data.Entities
{
    [Table("user_role")]
    public class UserRole
    {
        [Column("id")]
        private int Id { get; set; }  

        [Column("name")]
        private string RoleName { get; set; }

        [Column("deleted")]
        private bool isDeleted { get; set; }    
    }
}
