using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCare.Domain.Models.ModelsForUpdate
{
    public class UpdateOperationDomainModel
    {
        public decimal OldOperationId { get; set; }

        public decimal NewDoctorId { get; set; }

        public decimal NewPatientId { get; set; }

        public decimal NewDuration { get; set; }

        public DateTime NewStartTime { get; set; }

    }
}
