using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCare.Domain.Models.ModelsForUpdate
{
    public class UpdateRoomDomainModel
    {
        public decimal Id { get; set; }

        public string RoomName { get; set; }

        public decimal RoomTypeId { get; set; }

        public bool IsDeleted { get; set; }
    }
}
