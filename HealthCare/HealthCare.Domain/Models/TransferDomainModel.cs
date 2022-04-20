using HealthCare.Data.Entities;

namespace HealthCare.Domain.Models;

public class TransferDomainModel
{
    public decimal RoomFromId { get; set; }

    public decimal EquipmentId { get; set; }

    public decimal RoomToId { get; set; }

    public decimal Amount { get; set; }

    public DateTime TransferTime { get; set; }

    public bool isDeleted { get; set; }

    public Equipment Equipment { get; set; }

    public Room RoomFrom { get; set; }
    public Room RoomTo { get; set; }
}