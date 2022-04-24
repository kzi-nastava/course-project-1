using HealthCare.Data.Entities;

namespace HealthCare.Domain.Models;

public class ExaminationApprovalDomainModel
{
    public decimal Id { get; set; }
    public string State { get; set; }

    public decimal OldRoomId { get; set; }

    public decimal OldDoctorId { get; set; }

    public DateTime OldStartTime { get; set; }

    public decimal OldPatientId { get; set; }

    public decimal NewRoomId { get; set; }

    public decimal NewDoctorId { get; set; }

    public DateTime NewStartTime { get; set; }

    public decimal NewPatientId { get; set; }

    public bool isDeleted { get; set; }

    //public ExaminationDomainModel Examination { get; set; }

}