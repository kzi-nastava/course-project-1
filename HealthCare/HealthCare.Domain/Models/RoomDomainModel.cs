using HealthCare.Data.Entities;

namespace HealthCare.Domain.Models;

public class RoomDomainModel
{
    public decimal Id { get; set; }

    public string RoomName { get; set; }

    public decimal RoomTypeId  { get; set; }

    public bool IsDeleted { get; set; }

    public List<InventoryDomainModel> Inventories { get; set; }
    //public List<TransferDomainModel> TransfersFrom { get; set; }
    //public List<TransferDomainModel> TransfersTo { get; set; }
    public RoomTypeDomainModel RoomType { get; set; }
    public List<OperationDomainModel> Operations { get; set; }
}