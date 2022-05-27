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
        IDrugIngredientRepository _drugIngredientRepository;
        IDrugRepository _drugRepository;

        public DrugSuggestionService(IDrugSuggestionRepository drugSuggestionRepository, IDrugIngredientRepository drugIngredientRepository, IDrugRepository drugRepository)
        {
            _drugSuggestionRepository = drugSuggestionRepository;
            _drugIngredientRepository = drugIngredientRepository;
            _drugRepository = drugRepository;
        }

        public Task<DrugSuggestionDomainModel> Create(DrugSuggestionDTO drugSuggestionDTO)
        {
            throw new NotImplementedException();
        }

        public Task<DrugSuggestionDomainModel> Delete(decimal drugSuggestionId)
        {
            throw new NotImplementedException();
        }

        public void ApproveDrugIngredients(DrugSuggestion suggestion)
        {
            foreach (DrugIngredient drugIngredient in suggestion.Drug.DrugIngredients)
            {
                drugIngredient.IsDeleted = false;
            }

            _drugIngredientRepository.Save();
        }

        public async Task<DrugSuggestionDomainModel> Approve(decimal drugSuggestionId)
        {
            DrugSuggestion suggestion = await _drugSuggestionRepository.GetById(drugSuggestionId);

            suggestion.State = "approved";

            suggestion.Drug.IsDeleted = false;
            _drugRepository.Save();

            ApproveDrugIngredients(suggestion);

            return parseToModel(suggestion);

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

        public async Task<IEnumerable<DrugSuggestionDomainModel>> GetPending()
        {
            IEnumerable<DrugSuggestion> suggestions = await _drugSuggestionRepository.GetPending();
            List<DrugSuggestionDomainModel> result = new List<DrugSuggestionDomainModel>();
            foreach (DrugSuggestion item in suggestions)
            {
                result.Add(parseToModel(item));
            }
            return result;
        }

        private DrugSuggestionDomainModel parseToModel(DrugSuggestion suggestion)
        {
            DrugSuggestionDomainModel model =  new DrugSuggestionDomainModel
            {
                Id = suggestion.Id,
                Comment = suggestion.Comment,
                DrugId = suggestion.DrugId,
                State = TranslateState(suggestion.State),
   
            };

            if (suggestion.Drug != null)
                model.Drug = DrugService.ParseToModel(suggestion.Drug);

            return model;
        }

        private DrugSuggestionState TranslateState(string state)
        {
            switch (state)
            {
                case "created": return DrugSuggestionState.CREATED; break;
                case "approved": return DrugSuggestionState.APPROVED; break;
                case "for revision": return DrugSuggestionState.FOR_REVISION; break;
                default: throw new Exception("Undefined drug suggestion state");
            }
        }

    }
}
