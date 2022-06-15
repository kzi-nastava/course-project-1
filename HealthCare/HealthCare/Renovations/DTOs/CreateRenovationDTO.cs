using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCare.Domain.DTOs
{
    public class RenovationDTO
    {
        public decimal Id { get; set; }
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

    }
    public class CreateRenovationDTO
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

    }
    public class SimpleRenovationDTO : RenovationDTO
    {
        public decimal RoomId { get; set; }
    }
    public class CreateSimpleRenovationDTO : CreateRenovationDTO
    {
        public decimal RoomId { get; set; }
    }
    public class CreateJoinRenovationDTO : CreateRenovationDTO
    {
        public string resultRoomName { get; set; }
        public decimal roomTypeId { get; set; }
        public decimal JoinRoomId1 { get; set; }
        public decimal JoinRoomId2 { get; set; }
    }
    public class CreateSplitRenovationDTO : CreateRenovationDTO 
    {
        public string resultRoomName1 { get; set; }
        public decimal roomTypeId1 { get; set; }
        public string resultRoomName2 { get; set; }
        public decimal roomTypeId2 { get; set; }

        public decimal SplitRoomId { get; set; }

    }
}
