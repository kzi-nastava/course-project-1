using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthCare.Domain.Models;

namespace HealthCare.Domain.Interfaces {
    public interface ICredentialsService : IService<CredentialsDomainModel> {
        Task<CredentialsDomainModel> GetCredentialsByUsernameAndPassword(string username, string password);
    }
}
