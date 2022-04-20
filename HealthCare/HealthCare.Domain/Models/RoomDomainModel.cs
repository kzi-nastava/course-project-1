using HealthCare.Data.Entities;

namespace HealthCare.Domain.Models;

public class RoomDomainModel
{
    public decimal Id { get; set; }

    public string RoleName { get; set; }

    public decimal RoomTypeId  { get; set; }

    public bool isDeleted { get; set; }

    public List<Inventory> Inventories { get; set; }
    public List<Transfer> TransfersFrom { get; set; }
    public List<Transfer> TransfersTo { get; set; }
    public RoomType RoomType { get; set; }
    public List<Operation> Operations { get; set; }
}