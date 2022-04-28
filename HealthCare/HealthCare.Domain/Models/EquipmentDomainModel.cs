using HealthCare.Data.Entities;

namespace HealthCare.Domain.Models;

public class EquipmentDomainModel
{
    // ???
    private Equipment item;


    public decimal Id { get; set; }

    public string Name { get; set; }

    public decimal equipmentTypeId { get; set; }

    public bool IsDeleted { get; set; }

    public EquipmentTypeDomainModel EquipmentType { get; set; }

    //public List<TransferDomainModel> Transfers { get; set; }

    //public List<InventoryDomainModel> Inventories { get; set; }
}