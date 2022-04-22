using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCare.Data.Entities
{
    [Table("transfer")]
    public class Transfer
    {
        [Column("room_id1")]
        public decimal RoomFromId { get; set; }

        [Column("equipment_id")]
        public decimal EquipmentId { get; set; }

        [Column("room_id")]
        public decimal RoomToId { get; set; }

        [Column("amount")]
        public decimal Amount { get; set; }

        [Column("time_transfered")]
        public DateTime TransferTime { get; set; }

        [Column("deleted")]
        public bool isDeleted { get; set; }

        public Equipment Equipment { get; set; }

        //public Room RoomFrom { get; set; }
        //public Room RoomTo { get; set; }
    }
}
