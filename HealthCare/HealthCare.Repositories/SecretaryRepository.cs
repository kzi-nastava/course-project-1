using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthCare.Data.Context;
using HealthCare.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthCare.Repositories {
    public interface ISecretaryRepository : IRepository<Secretary> {

    }
    public class SecretaryRepository : ISecretaryRepository {
        private readonly HealthCareContext _healthCareContext;

        public SecretaryRepository(HealthCareContext healthCareContext) {
            _healthCareContext = healthCareContext;
        }
        public async Task<IEnumerable<Secretary>> GetAll() {
            return await _healthCareContext.Secretaries
                .Include(x => x.Credentials)
                .ToListAsync();
        }

        public void Save()
        {
            _healthCareContext.SaveChanges();
        }
    }
}
