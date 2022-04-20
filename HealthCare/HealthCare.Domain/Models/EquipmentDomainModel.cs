using HealthCare.Data.Entities;

namespace HealthCare.Domain.Models;

public class EquipmentDomainModel
{
    public decimal Id { get; set; }

    public string Name { get; set; }

    public decimal equipmentTypeId { get; set; }

    public bool IsDeleted { get; set; }

    public EquipmentType EquipmentType { get; set; }

    public List<Transfer> Transfers { get; set; }

    public List<Inventory> Inventories { get; set; }
}