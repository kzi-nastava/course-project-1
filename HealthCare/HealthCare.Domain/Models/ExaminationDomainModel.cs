using HealthCare.Data.Entities;

namespace HealthCare.Domain.Models;

public class ExaminationDomainModel
{

    public decimal Id { get; set; }

    public decimal DoctorId { get; set; }


    public decimal RoomId { get; set; }


    public decimal PatientId { get; set; }

    public DateTime StartTime { get; set; }

    public bool IsDeleted { get; set; }
    public AnamnesisDomainModel? Anamnesis { get; set; }
}