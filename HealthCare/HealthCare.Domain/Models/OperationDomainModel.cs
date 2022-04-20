using HealthCare.Data.Entities;

namespace HealthCare.Domain.Models;

public class OperationDomainModel
{
    public DateTime Duration { get; set; }

    public decimal RoomId  { get; set; }   

    public decimal DoctorId { get; set; }

    public decimal PatientId { get; set; }

    public bool isDeleted { get; set; }

    public Room Room { get; set; }

    public Patient Patient { get; set; }
    public Doctor Doctor { get; set; }
}