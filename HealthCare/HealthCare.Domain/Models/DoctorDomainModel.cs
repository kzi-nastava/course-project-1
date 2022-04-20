using HealthCare.Data.Entities;

namespace HealthCare.Domain.Models;

public class DoctorDomainModel
{
    public decimal Id { get; set; }

    public string Name { get; set; }

    public string Surname { get; set; }

    public string Email { get; set; }

    public DateTime BirthDate { get; set; }

    public string Phone { get; set; }

    public bool isDeleted { get; set; }

    public List<Operation> Operations { get; set; }

    public List<Examination> Examinations { get; set; }

    public Credentials Credentials { get; set; }
}