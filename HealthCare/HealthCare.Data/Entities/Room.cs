using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCare.Data.Entities
{
    [Table("room")]
    internal class Room
    {
        [Column("id")]
        private int Id { get; set; }

        [Column("name")]
        private string RoleName { get; set; }

        [Column("Room_type_id")]
        private int RoomTypeId  { get; set; }

        [Column("deleted")]
        private bool isDeleted { get; set; }
    }
}
