using HealthCare.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCare.Domain.Models
{
    public class AverageCountEvaluationDomainModel
    {
        public Question Question{ get; set; }
        public int Count { get; set; }
        public decimal Average { get; set; }
    }
}
