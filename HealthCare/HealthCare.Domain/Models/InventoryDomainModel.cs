using HealthCare.Data.Entities;

namespace HealthCare.Domain.Models;

public class InventoryDomainModel
{
    public decimal Amount { get; set; }

    public decimal roomId { get; set; }

    public decimal equipmentId { get; set; }

    public bool IsDeleted { get; set; }

    public Equipment Equipment { get; set; }

    public Room Room { get; set; }

}