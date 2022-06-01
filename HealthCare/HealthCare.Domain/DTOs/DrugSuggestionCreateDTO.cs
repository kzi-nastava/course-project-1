using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCare.Domain.DTOs
{
    public class DrugSuggestionCreateDTO
    {
        public DrugDTO DrugDTO { get; set; }

        public Dictionary<decimal, decimal> IngredientAmmounts{ get; set; }

    }
}
