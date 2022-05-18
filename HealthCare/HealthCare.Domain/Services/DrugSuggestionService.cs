using HealthCare.Data.Entities;
using HealthCare.Domain.DTOs;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using HealthCare.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCare.Domain.Services
{
    public class DrugSuggestionService : IDrugSuggestionService
    {
        IDrugSuggestionRepository _drugSuggestionRepository;

        public DrugSuggestionService(IDrugSuggestionRepository drugSuggestionRepository)
        {
            _drugSuggestionRepository = drugSuggestionRepository;
        }

        public Task<DrugSuggestionDomainModel> Create(DrugSuggestionDTO drugSuggestionDTO)
        {
            throw new NotImplementedException();
        }

        public Task<DrugSuggestionDomainModel> Delete(decimal drugSuggestionId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<DrugSuggestionDomainModel>> GetAll(){
            IEnumerable<DrugSuggestion> suggestions =  await _drugSuggestionRepository.GetAll();
            if(suggestions == null)
            {
                return new List<DrugSuggestionDomainModel>();
            }
            List<DrugSuggestionDomainModel> result = new List<DrugSuggestionDomainModel>();
            foreach (var suggestion in suggestions)
            {
                result.Add(parseToModel(suggestion));
            }
            return result;
        }

        private DrugSuggestionDomainModel parseToModel(DrugSuggestion suggestion)
        {
            return new DrugSuggestionDomainModel
            {
                Id = suggestion.Id,
                Comment = suggestion.Comment,
                DrugId = suggestion.DrugId,
                State = suggestion.State
            };

        }

    }
}
