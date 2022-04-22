using HealthCare.Data.Entities;

namespace HealthCare.Domain.Models;

public class RoomTypeDomainModel
{
    public decimal Id { get; set; }

    public string RoleName { get; set; }

    public bool isDeleted { get; set; }

    public string Purpose { get; set; }

   // public List<RoomDomainModel> Rooms { get; set; }
}