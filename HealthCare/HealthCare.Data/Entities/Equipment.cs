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
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("Equimpent_type")]
        public int equipmentTypeId { get; set; }

        [Column("deleted")]
        public bool IsDeleted { get; set; }

    }
}
