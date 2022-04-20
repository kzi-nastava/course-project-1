using HealthCare.Data.Entities;

namespace HealthCare.Domain.Models;

public class RoomTypeDomainModel
{
    public decimal Id { get; set; }

    public string RoleName { get; set; }

    public bool isDeleted { get; set; }

    public List<Room> Rooms { get; set; }
}