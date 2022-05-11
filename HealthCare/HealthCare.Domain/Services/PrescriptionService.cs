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
    public class PrescriptionService : IPrescriptionService
    {
        IPrescriptionRepository _prescriptionRepository;

        public PrescriptionService(IPrescriptionRepository prescriptionRepository)
        {
            _prescriptionRepository = prescriptionRepository;
        }    

        public async Task<PrescriptionDomainModel> Create(PrescriptionDomainModel model)
        {
            return model;
        }

        public async Task<IEnumerable<PrescriptionDomainModel>> GetAll()
        {
            IEnumerable<Prescription> data = await _prescriptionRepository.GetAll();
            if (data == null)
                return new List<PrescriptionDomainModel>();

            List<PrescriptionDomainModel> results = new List<PrescriptionDomainModel>();
            foreach (Prescription item in data)
            {
                results.Add(parseToModel(item));
            }

            return results;
        }

        private PrescriptionDomainModel parseToModel(Prescription prescription)
        {
            PrescriptionDomainModel prescriptionModel = new PrescriptionDomainModel
            {
                Id = prescription.Id,
                DrugId = prescription.DrugId,
                PatientId = prescription.PatientId,
                DoctorId = prescription.DoctorId,
                TakeAt = prescription.TakeAt,
                PerDay = prescription.PerDay,
                IsDeleted = prescription.IsDeleted,
                MealCombination = (MealCombination)Enum.Parse(typeof(MealCombination), prescription.MealCombination)
            };

            if (prescription.Drug != null)
            {
                prescriptionModel.Drug = new DrugDomainModel
                {
                    Id = prescription.Drug.Id,
                    Name = prescription.Drug.Name,
                    IsDeleted = prescription.Drug.IsDeleted
                };
            }

            return prescriptionModel;
        }
    }
}
