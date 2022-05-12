using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthCare.Data.Entities;
using HealthCare.Domain.DTOs;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using HealthCare.Repositories;

namespace HealthCare.Domain.Services 
{
    public class CredentialsService : ICredentialsService
    {

        private ICredentialsRepository _credentialsRepository;
        private IPatientRepository _patientRepository;

        public CredentialsService(ICredentialsRepository credentialsRepository, IPatientRepository patientRepository) 
        {
            _credentialsRepository = credentialsRepository;
            _patientRepository = patientRepository;
        }
        
        // Async awaits info from database
        // GetAll is the equivalent of SELECT *
        public async Task<IEnumerable<CredentialsDomainModel>> ReadAll()
        {
            IEnumerable<CredentialsDomainModel> credentials = await GetAll();
            List<CredentialsDomainModel> result = new List<CredentialsDomainModel>();
            foreach (var item in credentials)
            {
                if (!item.IsDeleted) result.Add(item);
            }
            return result;
        }
        public async Task<IEnumerable<CredentialsDomainModel>> GetAll()
        {
            IEnumerable<Credentials> data = await _credentialsRepository.GetAll();
            if (data == null)
                return new List<CredentialsDomainModel>();

            List<CredentialsDomainModel> results = new List<CredentialsDomainModel>();
            CredentialsDomainModel credentialsModel;
            foreach (Credentials item in data)
            {
                credentialsModel = new CredentialsDomainModel {
                    Id = item.Id,
                    Username = item.Username,
                    Password = item.Password,
                    DoctorId = item.DoctorId,
                    SecretaryId = item.SecretaryId,
                    ManagerId = item.ManagerId,
                    PatientId = item.PatientId,
                    UserRoleId = item.UserRoleId,
                    IsDeleted = item.isDeleted
                };
                if (item.UserRole != null) {
                    credentialsModel.UserRole = new UserRoleDomainModel {
                        Id = item.UserRole.Id,
                        RoleName = item.UserRole.RoleName,
                        IsDeleted = item.UserRole.IsDeleted
                    };
                }
                results.Add(credentialsModel);
            }
            
            return results;
        }

        public async Task<Boolean> IsBlocked(CredentialsDomainModel credentialsModel)
        {
            Patient patient = await _patientRepository.GetPatientById(credentialsModel.PatientId.GetValueOrDefault());
            if (patient.BlockedBy.Equals("")) return false;
            return true;
        }

        public async Task<CredentialsDomainModel> GetCredentialsByUsernameAndPassword(LoginDTO dto)
        {
            IEnumerable<CredentialsDomainModel> data = await ReadAll();
            foreach (CredentialsDomainModel item in data) {
                if (item.Username.Equals(dto.Username) && item.Password.Equals(dto.Password))
                {
                    if (item.PatientId != null)
                    {
                        if (!await IsBlocked(item)) return item;
                        throw new UserIsBlockedException();
                    }
                    return item;
                }
            }
            throw new UserNotFoundException();
        }
    }
}
