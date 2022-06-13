using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCare.Data.Entities
{
    public class AverageCountEvaluation
    {
        public Question Question{ get; set; }
        public int Count { get; set; }
        public decimal Average { get; set; }
    }
}
