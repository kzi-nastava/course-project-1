using HealthCare.Data.Entities;

namespace HealthCare.Domain.Models;

public class ExaminationDomainModel
{
    public decimal doctorId { get; set; }

    public decimal roomId { get; set; }

    public decimal patientId { get; set; }

    public DateTime StartTime { get; set; }

    public bool IsDeleted { get; set; }
    public Anamnesis Anamnesis { get; set; }
    public ExaminationApproval ExaminationApproval { get; set; }
    public Patient Patient { get; set; }
    public Doctor Doctor { get; set; } 
}