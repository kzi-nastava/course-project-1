using HealthCare.Data.Entities;
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
    public class ReferralLetterService : IReferralLetterService
    {
        private IReferralLetterRepository _referralLetterRepository;

        public ReferralLetterService(IReferralLetterRepository referralLetterRepository)
        {
            _referralLetterRepository = referralLetterRepository; 
        }

        private ReferralLetterDomainModel parseToModel(ReferralLetter referralLetter)
        {
            ReferralLetterDomainModel referralLetterModel = new ReferralLetterDomainModel
            {
                Id = referralLetter.Id,
                FromDoctorId = referralLetter.FromDoctorId,
                ToDoctorId = referralLetter.ToDoctorId,
                PatientId = referralLetter.PatientId
            };
            return referralLetterModel;
        }

        private ReferralLetter parseFromModel(ReferralLetterDomainModel referralLetterModel)
        {
            ReferralLetter referralLetter = new ReferralLetter
            {
                Id = referralLetterModel.Id,
                FromDoctorId = referralLetterModel.FromDoctorId,
                ToDoctorId = referralLetterModel.ToDoctorId,
                PatientId = referralLetterModel.PatientId
            };
            return referralLetter;
        }

        public async Task<IEnumerable<ReferralLetterDomainModel>> GetAll()
        {
            IEnumerable<ReferralLetter> data = await _referralLetterRepository.GetAll();
            if (data == null)
                return new List<ReferralLetterDomainModel>();

            List<ReferralLetterDomainModel> results = new List<ReferralLetterDomainModel>();
            foreach (ReferralLetter item in data)
            {
                results.Add(parseToModel(item));
            }

            return results;
        }
    }
}
