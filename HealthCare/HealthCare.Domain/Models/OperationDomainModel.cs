using HealthCare.Data.Entities;

namespace HealthCare.Domain.Models;

public class OperationDomainModel
{
    public decimal Id { get; set; }

    public DateTime StartTime { get; set; }

    public decimal Duration { get; set; }

    public decimal RoomId { get; set; }

    public decimal DoctorId { get; set; }

    public decimal PatientId { get; set; }

    public bool IsDeleted { get; set; }
    public bool IsEmergency { get; set; }

}