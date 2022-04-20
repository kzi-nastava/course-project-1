using HealthCare.Data.Entities;

namespace HealthCare.Domain.Models;

public class MedicalRecordDomainModel
{
    public decimal Height { get; set; }

    public decimal Weight { get; set; }

    public string BedriddenDiseases { get; set; }

    public string Allergies { get; set; }

    public decimal PatientId { get; set; }

    public bool isDeleted { get; set; }

    public Patient Patient { get; set; }
}