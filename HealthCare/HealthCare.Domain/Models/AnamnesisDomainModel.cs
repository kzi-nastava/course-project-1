using System.ComponentModel.DataAnnotations.Schema;
using HealthCare.Data.Entities;

namespace HealthCare.Domain.Models;

public class AnamnesisDomainModel {
    public string Description { get; set; }

    public decimal doctorId { get; set; }

    public decimal roomId { get; set; }

    public decimal patientId { get; set; }

    public DateTime StartTime { get; set; }

    public bool isDeleted { get; set; }

    public Examination Examination { get; set; } 
}