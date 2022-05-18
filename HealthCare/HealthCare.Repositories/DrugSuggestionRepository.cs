﻿using HealthCare.Data.Context;
using HealthCare.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCare.Repositories
{
    public interface IDrugSuggestionRepository : IRepository<DrugSuggestion>
    {
        public DrugSuggestion Post(DrugSuggestion allergy);
        public DrugSuggestion Update(DrugSuggestion allergy);

        public Task<DrugSuggestion> GetById(decimal id);
    }
    public class DrugSuggestionRepository : IDrugSuggestionRepository
    {
        private readonly HealthCareContext _healthCareContext;

        public DrugSuggestionRepository(HealthCareContext healthCareContext)
        {
            _healthCareContext = healthCareContext;
        }
        public async Task<IEnumerable<DrugSuggestion>> GetAll()
        {
            return await _healthCareContext.DrugSuggestions.ToListAsync();
        }

        public async Task<DrugSuggestion> GetById(decimal id)
        {
            return await _healthCareContext.DrugSuggestions.FindAsync(id);
        }

        public DrugSuggestion Post(DrugSuggestion newDrugSuggestion)
        {
            EntityEntry<DrugSuggestion> result = _healthCareContext.Add(newDrugSuggestion);
            return result.Entity;
        }

        public void Save()
        {
            _healthCareContext.SaveChanges();
        }

        public DrugSuggestion Update(DrugSuggestion updatedDrugSuggestion)
        {
            EntityEntry<DrugSuggestion> result = _healthCareContext.Attach(updatedDrugSuggestion);
            result.State = EntityState.Modified;
            return result.Entity;

        }
    }
}
