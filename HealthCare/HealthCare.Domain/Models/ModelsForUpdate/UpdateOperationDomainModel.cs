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

        public decimal newDoctorId { get; set; }

        public decimal newRoomId { get; set; }

        public decimal newPatientId { get; set; }

        public decimal newDuration { get; set; }

        public DateTime newStartTime { get; set; }

    }
}
