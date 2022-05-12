using HealthCare.Data.Entities;
using HealthCare.Domain.DTOs;
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
        IExaminationRepository _examinationRepository;
        IMedicalRecordRepository _medicalRecordRepository;
        IDrugRepository _drugRepository;
        IIngredientRepository _ingredientRepository;

        public PrescriptionService(IPrescriptionRepository prescriptionRepository, IExaminationRepository examinationRepository, 
                                   IMedicalRecordRepository medicalRecordRepository, IDrugRepository drugRepository, IIngredientRepository ingredientRepository)
        {
            _prescriptionRepository = prescriptionRepository;
            _examinationRepository = examinationRepository;
            _medicalRecordRepository = medicalRecordRepository;
            _drugRepository = drugRepository;
            _ingredientRepository = ingredientRepository;
        }    

        public async Task<PrescriptionDomainModel> Create(PrescriptionDTO prescriptionDTO)
        {
            await checkPatientsAllergies(prescriptionDTO.DrugId, prescriptionDTO.PatientId);

            Prescription newPrescription = _prescriptionRepository.Post(parseFromDTO(prescriptionDTO));
            _prescriptionRepository.Save();

            return ParseToModel(newPrescription);
        }

        private async Task checkPatientsAllergies(decimal drugId, decimal patientId)
        {
            Drug drug = await _drugRepository.GetById(drugId);
            MedicalRecord medicalRecord = await _medicalRecordRepository.GetByPatientId(patientId);

            foreach (Allergy allergy in medicalRecord.AllergiesList)
            {
                foreach (DrugIngredient drugIngredient in drug.DrugIngredients)
                {
                    if (allergy.IngredientId == drugIngredient.IngredientId)
                    {
                        Ingredient allergen = await _ingredientRepository.GetById(allergy.IngredientId);
                        throw new PatientIsAllergicException(allergen.Name);
                    }
                        
                }
            }

        }

        public async Task<IEnumerable<PrescriptionDomainModel>> GetAll()
        {
            IEnumerable<Prescription> data = await _prescriptionRepository.GetAll();
            if (data == null)
                return new List<PrescriptionDomainModel>();

            List<PrescriptionDomainModel> results = new List<PrescriptionDomainModel>();
            foreach (Prescription item in data)
            {
                results.Add(ParseToModel(item));
            }

            return results;
        }

        public static Prescription parseFromDTO(PrescriptionDTO prescriptionDTO)
        {
            Prescription prescription = new Prescription
            {
                DoctorId = prescriptionDTO.DoctorId,
                PatientId = prescriptionDTO.PatientId,
                DrugId = prescriptionDTO.DrugId,
                TakeAt = prescriptionDTO.TakeAt,
                PerDay = prescriptionDTO.PerDay,    
                MealCombination = prescriptionDTO.MealCombination
            };

            return prescription;
        }

        private DateTime removeSeconds(DateTime dateTime)
        {
            int year = dateTime.Year;
            int month = dateTime.Month;
            int day = dateTime.Day;
            int hour = dateTime.Hour;
            int minute = dateTime.Minute;
            int second = 0;
            return new DateTime(year, month, day, hour, minute, second);
        }

        public static PrescriptionDomainModel ParseToModel(Prescription prescription)
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
                prescriptionModel.Drug = DrugService.ParseToModel(prescription.Drug);

            return prescriptionModel;
        }
        public static Prescription ParseFromModel(PrescriptionDomainModel prescriptionModel)
        {
            Prescription prescription = new Prescription
            {
                Id = prescriptionModel.Id,
                DrugId = prescriptionModel.DrugId,
                PatientId = prescriptionModel.PatientId,
                DoctorId = prescriptionModel.DoctorId,
                TakeAt = prescriptionModel.TakeAt,
                PerDay = prescriptionModel.PerDay,
                IsDeleted = prescriptionModel.IsDeleted,
                // TODO: Sta?
                //MealCombination = (MealCombination)Enum.Parse(typeof(MealCombination), prescriptionModel.MealCombination)
            };

            if (prescriptionModel.Drug != null)
                prescription.Drug = DrugService.ParseFromModel(prescriptionModel.Drug);

            return prescription;
        }
    }
    
}
