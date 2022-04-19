using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCare.Data.Entities
{
    [Table("inventory")]
    public class Inventory
    {
        [Column("amount")]
        public int Amount { get; set; }

        [Column("room_id")]
        public int roomId { get; set; }

        [Column("equipment_id")]
        public int equipmentId { get; set; }

        [Column("deleted")]
        public bool IsDeleted { get; set; }

    }
}
