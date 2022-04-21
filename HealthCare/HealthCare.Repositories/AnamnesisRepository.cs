﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthCare.Data.Context;
using HealthCare.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthCare.Repositories {
    public interface IAnamnesisRepository : IRepository<Anamnesis> {

    }
    public class AnamnesisRepository : IAnamnesisRepository {
        private readonly HealthCareContext _healthCareContext;

        public AnamnesisRepository(HealthCareContext healthCareContext) {
            _healthCareContext = healthCareContext;
        }
        public async Task<IEnumerable<Anamnesis>> GetAll() {
            return await _healthCareContext.Anamneses.ToListAsync();
        }

        public void Save()
        {
            _healthCareContext.SaveChanges();
        }
    }
}
