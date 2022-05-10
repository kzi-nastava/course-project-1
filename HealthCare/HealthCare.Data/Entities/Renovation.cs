using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCare.Data.Entities
{
    [Table("renovation")]
    public class Renovation
    {
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public decimal Id { get; set; }

        [Column("start_date")]
        public DateTime StartDate{ get; set; }

        [Column("end_date")]
        public DateTime EndDate { get; set; }

        [Column("split")]
        public bool Split { get; set; }

        [Column("room_id")]
        public decimal Participant { get; set; }

        [Column("room_id1")]
        public decimal Participant1 { get; set; }

        [Column("room_id2")]
        public decimal Participant2 { get; set; }

        public bool IsSimple()
        {
            return Participant1 == null || Participant2 == null || IsSplit == null;
        }

        public bool IsSplit()
        {
            if (Split == null)
                return false;

            return Split;
        }
    }
}
