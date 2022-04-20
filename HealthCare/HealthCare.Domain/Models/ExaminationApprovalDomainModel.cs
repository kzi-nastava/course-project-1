using HealthCare.Data.Entities;

namespace HealthCare.Domain.Models;

public class ExaminationApprovalDomainModel
{
    public string State { get; set; }

    public decimal RoomId { get; set; }

    public decimal DoctorId { get; set; }

    public DateTime StartTime { get; set; }

    public decimal PatientId { get; set; }

    public bool isDeleted { get; set; }

    public Examination Examination { get; set; }

}