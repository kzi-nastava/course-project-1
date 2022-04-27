using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthCare.Data.Entities;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using HealthCare.Repositories;

namespace HealthCare.Domain.Services {
    public class CredentialsService : ICredentialsService{

        private ICredentialsRepository _credentialsRepository;
        private IUserRoleRepository _userRoleRepository;
        private IManagerRepository _managerRepository;
        private IPatientRepository _patientRepository;
        private ISecretaryRepository _secretaryRepository;
        private IDoctorRepository _doctorRepository;

        public CredentialsService(ICredentialsRepository credentialsRepository) {
            _credentialsRepository = credentialsRepository;
        }
        
        // Async awaits info from database
        // GetAll is the equivalent of SELECT *
        public async Task<IEnumerable<CredentialsDomainModel>> ReadAll()
        {
            IEnumerable<CredentialsDomainModel> credentials = await GetAll();
            List<CredentialsDomainModel> result = new List<CredentialsDomainModel>();
            foreach (var item in credentials)
            {
                if(!item.isDeleted) result.Add(item);
            }
            return result;
        }
        public async Task<IEnumerable<CredentialsDomainModel>> GetAll()
        {
            var data = await _credentialsRepository.GetAll();
            if (data == null)
                return null;

            List<CredentialsDomainModel> results = new List<CredentialsDomainModel>();
            CredentialsDomainModel credentialsModel;
            foreach (var item in data)
            {
                credentialsModel = new CredentialsDomainModel {
                    Id = item.Id,
                    Username = item.Username,
                    Password = item.Password,
                    doctorId = item.doctorId,
                    secretaryId = item.secretaryId,
                    managerId = item.managerId,
                    patientId = item.patientId,
                    userRoleId = item.userRoleId,
                    isDeleted = item.isDeleted
                };
                if (item.UserRole != null) {
                    credentialsModel.UserRole = new UserRoleDomainModel {
                        Id = item.UserRole.Id,
                        RoleName = item.UserRole.RoleName,
                        isDeleted = item.UserRole.isDeleted
                    };
                }
                results.Add(credentialsModel);
            }
            
            return results;
        }

        // TODO: Fix this method in the future
        public async Task<CredentialsDomainModel> GetCredentialsByUsernameAndPassword(string username, string password)
        {
            var data = await GetAll();
            foreach (var item in data) {
                if (item.Username.Equals(username) && item.Password.Equals(password)) {
                    return item;
                }
            }
            return null;
        }
    }
}
