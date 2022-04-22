using HealthCare.Data.Entities;

namespace HealthCare.Domain.Models;

public class OperationDomainModel
{
    public DateTime Duration { get; set; }

    public decimal RoomId  { get; set; }   

    public decimal DoctorId { get; set; }

    public decimal PatientId { get; set; }

    public bool isDeleted { get; set; }

    //public RoomDomainModel Room { get; set; }

    //public PatientDomainModel Patient { get; set; }
    //public DoctorDomainModel Doctor { get; set; }
}