using HealthCare.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCare.Domain.Interfaces
{
    public interface IDrugSuggestionService
    {
        Task<IEnumerable<DrugSuggestionDomainModel>> GetAll();
        Task<DrugSuggestionDomainModel> Create(DTOs.DrugSuggestionDTO drugSuggestionDTO);
        Task<DrugSuggestionDomainModel> Delete(decimal drugSuggestionId);
    }
}
