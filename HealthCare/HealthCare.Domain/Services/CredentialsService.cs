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
        
        public static CredentialsDomainModel parseToModel(Credentials credentials)
        {
            CredentialsDomainModel credentialsModel = new CredentialsDomainModel
            {
                Id = credentials.Id,
                Username = credentials.Username,
                Password = credentials.Password,
                DoctorId = credentials.DoctorId,
                SecretaryId = credentials.SecretaryId,
                ManagerId = credentials.ManagerId,
                PatientId = credentials.PatientId,
                UserRoleId = credentials.UserRoleId,
                IsDeleted = credentials.isDeleted
            };
            if (credentials.UserRole != null)
            {
                credentialsModel.UserRole = UserRoleService.parseToModel(credentials.UserRole);
            }
            return credentialsModel;
        }
        
        public static Credentials parseFromModel(CredentialsDomainModel credentialsModel)
        {
            Credentials credentials = new Credentials 
            {
                Id = credentialsModel.Id,
                Username = credentialsModel.Username,
                Password = credentialsModel.Password,
                DoctorId = credentialsModel.DoctorId,
                SecretaryId = credentialsModel.SecretaryId,
                ManagerId = credentialsModel.ManagerId,
                PatientId = credentialsModel.PatientId,
                UserRoleId = credentialsModel.UserRoleId,
                isDeleted = credentialsModel.IsDeleted
            };
            if (credentialsModel.UserRole != null)
            {
                credentials.UserRole = UserRoleService.parseFromModel(credentialsModel.UserRole);
            }
            return credentials;
        }
    }
}
