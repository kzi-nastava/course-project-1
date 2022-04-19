using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCare.Data.Entities
{
    [Table("transfer")]
    internal class Transfer
    {
        [Column("room_id1")]
        private int RoomFromId { get; set; }

        [Column("equipment_id")]
        private int EquipmentId { get; set; }

        [Column("room_id")]
        private int RoomToId { get; set; }

        [Column("amount")]
        private int Amount { get; set; }

        [Column("time_transfered")]
        private DateTime TransferTime { get; set; }

        [Column("deleted")]
        private bool isDeleted { get; set; }
    }
}
