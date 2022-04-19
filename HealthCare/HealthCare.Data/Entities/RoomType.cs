using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCare.Data.Entities
{
    [Table("room_type")]
    internal class RoomType
    {
        [Column("id")]
        private int Id { get; set; }

        [Column("name")]
        private string RoleName { get; set; }

        [Column("deleted")]
        private bool isDeleted { get; set; }
    }
}
