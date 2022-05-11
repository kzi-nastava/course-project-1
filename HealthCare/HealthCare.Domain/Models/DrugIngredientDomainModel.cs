using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCare.Domain.Models
{
    public class DrugIngredientDomainModel
    {
        public decimal Id { get; set; }

        public decimal DrugId { get; set; }

        public decimal Amount { get; set; }
    }
}
