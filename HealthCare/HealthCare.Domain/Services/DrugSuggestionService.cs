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
        IDrugService _drugService;

        public DrugSuggestionService(IDrugSuggestionRepository drugSuggestionRepository, IDrugIngredientRepository drugIngredientRepository, IDrugRepository drugRepository, IDrugService drugService)
        {
            _drugSuggestionRepository = drugSuggestionRepository;
            _drugIngredientRepository = drugIngredientRepository;
            _drugRepository = drugRepository;
            _drugService = drugService;
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

            if (suggestion.State == "approved")
                throw new DrugSuggestionAlreadyApprovedException();

            suggestion.State = "approved";

            suggestion.Drug.IsDeleted = false;
            _drugRepository.Save();

            ApproveDrugIngredients(suggestion);

            return ParseToModel(suggestion);

        }

        public async Task<DrugSuggestionDomainModel> Revision(decimal drugSuggestionId, string comment)
        {
            DrugSuggestion suggestion = await _drugSuggestionRepository.GetById(drugSuggestionId);

            suggestion.State = "revision";
            suggestion.Comment = comment;

            _drugIngredientRepository.Save();

            return ParseToModel(suggestion);
        }
        public async Task<DrugSuggestionDomainModel> Reject(decimal drugSuggestionId, string comment)
        {
            DrugSuggestion suggestion = await _drugSuggestionRepository.GetById(drugSuggestionId);

            if (suggestion.State == "rejected")
                throw new DrugSuggestionAlreadyRejectedException();

            suggestion.State = "rejected";
            suggestion.Comment = comment;

            _drugIngredientRepository.Save();

            return ParseToModel(suggestion);
        }

        public async Task<IEnumerable<DrugSuggestionDomainModel>> GetAll(){
            IEnumerable<DrugSuggestion> suggestions =  await _drugSuggestionRepository.GetAll();
            if(suggestions == null)
            {
                return new List<DrugSuggestionDomainModel>();
            }
            return ParseToModel(suggestions);
        }

        public async Task<IEnumerable<DrugSuggestionDomainModel>> GetPending()
        {
            IEnumerable<DrugSuggestion> suggestions = await _drugSuggestionRepository.GetPending();
            return ParseToModel(suggestions);
        }

        public async Task<IEnumerable<DrugSuggestionDomainModel>> GetRejected()
        {
            IEnumerable<DrugSuggestion> suggestions = await _drugSuggestionRepository.GetRejected();
            return ParseToModel(suggestions);
        }

        private IEnumerable<DrugSuggestionDomainModel> ParseToModel(IEnumerable<DrugSuggestion> suggestions)
        {
            List<DrugSuggestionDomainModel> result = new List<DrugSuggestionDomainModel>();
            foreach (DrugSuggestion item in suggestions)
            {
                result.Add(ParseToModel(item));
            }
            return result;
        }

        private DrugSuggestionDomainModel ParseToModel(DrugSuggestion suggestion)
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
                case "revision": return DrugSuggestionState.REVISION; break;
                case "approved": return DrugSuggestionState.APPROVED; break;              
                case "rejected": return DrugSuggestionState.REJECTED; break;
                default: throw new Exception("Undefined drug suggestion state");
            }
        }

        public async Task<DrugSuggestionDomainModel> Update(DrugSuggestionUpdateDTO dto)
        {
            DrugSuggestion drugSuggestion = new DrugSuggestion
            {
                DrugId = dto.DrugId,
                Comment = dto.Comment,
                State = dto.State,
            };
            _drugSuggestionRepository.Update(drugSuggestion);
            _drugSuggestionRepository.Save();
            return ParseToModel(drugSuggestion);
        }

        public DrugSuggestionDomainModel Create(DrugSuggestionCreateDTO drugSuggestionDTO)
        {
            DrugDomainModel drug = _drugService.Create(drugSuggestionDTO.DrugDTO);

            foreach(KeyValuePair<decimal, decimal> ingredientAmount in drugSuggestionDTO.IngredientAmounts)
            {
                DrugIngredient drugIngredient = new DrugIngredient
                {
                    DrugId = drug.Id,
                    IngredientId = ingredientAmount.Key,
                    Amount = ingredientAmount.Value,
                    IsDeleted = true,
                };
                _drugIngredientRepository.Post(drugIngredient);
            }
            _drugIngredientRepository.Save();

            DrugSuggestion drugSuggestion = new DrugSuggestion
            {
                Comment = null,
                State = "created",
                DrugId = drug.Id,
            };
            _drugSuggestionRepository.Post(drugSuggestion);
            _drugSuggestionRepository.Save();
            return ParseToModel(drugSuggestion);

            
        }
    }
}
