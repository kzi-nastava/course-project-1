using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCare.Domain.Models.ModelsForCreate
{
    public class CreateOperationDomainModel
    {
        public DateTime StartTime { get; set; }
        public decimal Duration { get; set; }
        public decimal DoctorId { get; set; }
        public decimal PatientId { get; set; }
    }
}
