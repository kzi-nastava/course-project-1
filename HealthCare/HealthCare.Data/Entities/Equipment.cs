using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCare.Data.Entities
{
    [Table("equipment")]
    public class Equipment
    {
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("Equipment_type_id")]
        public decimal equipmentTypeId { get; set; }

        [Column("deleted")]
        public bool IsDeleted { get; set; }

        public EquipmentType EquipmentType { get; set; }

        //public List<Transfer> Transfers { get; set; }

        //public List<Inventory> Inventories { get; set; }





    }
}
