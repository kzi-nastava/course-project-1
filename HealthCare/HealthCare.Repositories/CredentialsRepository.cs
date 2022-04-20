using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthCare.Data.Context;
using HealthCare.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthCare.Repositories
{

    public interface ICredentialsRepository : IRepository<Credentials> {

    }
    public class CredentialsRepository : ICredentialsRepository
    {
        private readonly HealthCareContext _healthCareContext;

        public CredentialsRepository(HealthCareContext healthCareContext)
        {
            _healthCareContext = healthCareContext;
        }
        public async Task<IEnumerable<Credentials>> GetAll()
        {
            return await _healthCareContext.Credentials.ToListAsync();
        }
    }
}
