using HealthCare.Data.Entities;

namespace HealthCare.Domain.Models;

public class ExaminationDomainModel
{

    public decimal Id { get; set; }

    public decimal doctorId { get; set; }


    public decimal roomId { get; set; }


    public decimal patientId { get; set; }

    public DateTime StartTime { get; set; }

    public bool IsDeleted { get; set; }
    public AnamnesisDomainModel? Anamnesis { get; set; }
    //public ExaminationApprovalDomainModel? ExaminationApproval { get; set; }
    //public Patient Patient { get; set; }
    //public Doctor Doctor { get; set; } 
}