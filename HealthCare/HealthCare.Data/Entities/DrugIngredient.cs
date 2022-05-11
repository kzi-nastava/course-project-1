using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCare.Data.Entities
{
    [Table("drug-ingredient")]
    public class DrugIngredient
    {
        [Column("ingredient_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal IngredientId { get; set; }

        [Column("drug_id")]
        public decimal DrugId { get; set; }

        [Column("amount")]
        public decimal Amount { get; set; }

        public Ingredient Ingredient { get; set; }
        
        public Drug Drug { get; set; }
    }
}
