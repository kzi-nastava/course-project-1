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
        public Task<IEnumerable<DrugSuggestionDomainModel>> GetAll();
        public Task<DrugSuggestionDomainModel> Create(DTOs.DrugSuggestionDTO drugSuggestionDTO);
        public Task<DrugSuggestionDomainModel> Delete(decimal drugSuggestionId);
        public Task<IEnumerable<DrugSuggestionDomainModel>> GetPending();
        Task<DrugSuggestionDomainModel> Approve(decimal drugSuggestionId);
    }
}
