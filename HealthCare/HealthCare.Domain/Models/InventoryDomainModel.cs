using HealthCare.Data.Entities;

namespace HealthCare.Domain.Models;

public class InventoryDomainModel
{
    public decimal Amount { get; set; }

    public decimal roomId { get; set; }

    public decimal equipmentId { get; set; }

    public bool IsDeleted { get; set; }

    public EquipmentDomainModel Equipment { get; set; }

    //public RoomDomainModel Room { get; set; }

}