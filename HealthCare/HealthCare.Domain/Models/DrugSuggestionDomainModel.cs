using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCare.Domain.Models
{
    public enum DrugSuggestionState
    {
        CREATED,
        FOR_REVISION,
        APPROVED
    }
    public class DrugSuggestionDomainModel
    {
        public decimal Id { get; set; }

        public decimal DrugId { get; set; }

        public DrugSuggestionState State { get; set; }

        public string? Comment { get; set; }
    }
}
