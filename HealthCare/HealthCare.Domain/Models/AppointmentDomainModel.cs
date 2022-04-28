using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCare.Domain.Models
{
    public enum Appointment
    {
        Operation, 
        Examination
    }

    public class AppointmentDomainModel
    {
        public decimal Id { get; set; }
        public decimal DoctorId { get; set; }

        public decimal RoomId { get; set; }

        public decimal PatientId { get; set; }

        public DateTime StartTime { get; set; }

        public decimal Duration  { get; set; }

        public Appointment Type { get; set; }

        public bool IsDeleted { get; set; }
        public AnamnesisDomainModel? Anamnesis { get; set; }
    }
}
