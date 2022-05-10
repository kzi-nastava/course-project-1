using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCare.Domain.Models
{
    public class RenovationDomainModel
    {
        public decimal Id { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool Split { get; set; }

        public decimal Participant { get; set; }

        public decimal Participant1 { get; set; }

        public decimal Participant2 { get; set; }
    }
}
