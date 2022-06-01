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
    public class DrugService : IDrugService
    {
        IDrugRepository _drugRepository;

        public DrugService(IDrugRepository drugRepository)
        {
            _drugRepository = drugRepository;
        }

        public async Task<IEnumerable<DrugDomainModel>> GetAll()
        {
            IEnumerable<Drug> data = await _drugRepository.GetAll();
            if (data == null)
                return new List<DrugDomainModel>();

            List<DrugDomainModel> results = new List<DrugDomainModel>();
            foreach (Drug item in data)
            {
                results.Add(ParseToModel(item));
            }

            return results;
        }

        public static DrugDomainModel ParseToModel(Drug drug)
        {
            DrugDomainModel drugModel = new DrugDomainModel
            {
                Id = drug.Id,
                Name = drug.Name,
                IsDeleted = drug.IsDeleted
            };

            drugModel.DrugIngredients = new List<DrugIngredientDomainModel>();
            if (drug.DrugIngredients != null)
                foreach (DrugIngredient drugIngredient in drug.DrugIngredients)
                    drugModel.DrugIngredients.Add(DrugIngredientService.ParseToModel(drugIngredient));

            return drugModel;
        }
        
        public static Drug ParseFromModel(DrugDomainModel drugModel)
        {
            Drug drug = new Drug
            {
                Id = drugModel.Id,
                Name = drugModel.Name,
                IsDeleted = drugModel.IsDeleted
            };

            drugModel.DrugIngredients = new List<DrugIngredientDomainModel>();
            if (drugModel.DrugIngredients != null)
                foreach (DrugIngredientDomainModel drugIngredient in drugModel.DrugIngredients)
                    drug.DrugIngredients.Add(DrugIngredientService.ParseFromModel(drugIngredient));

            return drug;
        }

        public DrugDomainModel Create(DrugDTO dto)
        {
            Drug drug = new Drug
            {
                Name = dto.Name,
                IsDeleted = true,
            };
            _drugRepository.Post(drug);
            _drugRepository.Save();
            return ParseToModel(drug);
        }

        public async Task<DrugDomainModel> Update(decimal id, string name)
        {
            Drug drug = await _drugRepository.GetById(id);
            drug.Name = name;
            _drugRepository.Update(drug);
            _drugRepository.Save();
            return ParseToModel(drug);
        }
    }
}
