using HealthCare.Data.Entities;

namespace HealthCare.Domain.Models;

public class UserRoleDomainModel
{
    public decimal Id { get; set; }  

    public string RoleName { get; set; }

    public bool IsDeleted { get; set; }

    //public List<Credentials> Credentials { get; set; }
}