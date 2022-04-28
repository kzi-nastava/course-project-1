using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCare.Domain.Models.ModelsForCreate
{
    public class CreateAnamnesisDomainModel
    {
        public string Description { get; set; }

        public decimal ExaminationId { get; set; }
    }
}
