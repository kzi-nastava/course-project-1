using HealthCare.Domain.Models;
using HealthCare.Repositories;
using HealthCare.Data.Entities;

namespace HealthCare.Domain.Interfaces;

public class MedicalRecordService : IMedicalRecordService 
{
    private IMedicalRecordRepository _medicalRecordRepository;

    public MedicalRecordService(IMedicalRecordRepository medicalRecordRepository) 
    {
        _medicalRecordRepository = medicalRecordRepository;
    }

    public async Task<IEnumerable<MedicalRecordDomainModel>> ReadAll()
    {
        IEnumerable<MedicalRecordDomainModel> medicalRecords = await GetAll();
        List<MedicalRecordDomainModel> result = new List<MedicalRecordDomainModel>();
        foreach (MedicalRecordDomainModel item in medicalRecords)
        {
            if (!item.IsDeleted) result.Add(item);
        }
        return result;
    } 
    public async Task<IEnumerable<MedicalRecordDomainModel>> GetAll()
    {
        IEnumerable<MedicalRecord> data = await _medicalRecordRepository.GetAll();
        if (data == null)
            return new List<MedicalRecordDomainModel>();

        List<MedicalRecordDomainModel> results = new List<MedicalRecordDomainModel>();
        MedicalRecordDomainModel medicalRecordModel;
        foreach (MedicalRecord item in data)
        {
            results.Add(parseToModel(item));
        }

        return results;
    } 

    public async Task<MedicalRecordDomainModel> GetForPatient(decimal id)
    {
        MedicalRecord data =  await _medicalRecordRepository.GetByPatientId(id);

        if (data == null) 
            throw new DataIsNullException();
        
        return parseToModel(data);
    }

    public async Task<MedicalRecordDomainModel> Update(MedicalRecordDomainModel medicalRecordModel)
    {
        MedicalRecord medicalRecord = _medicalRecordRepository.Update(parseFromModel(medicalRecordModel));
        _medicalRecordRepository.Save();
        return parseToModel(medicalRecord);
    }

    private MedicalRecordDomainModel parseToModel(MedicalRecord medicalRecord)
    {
        MedicalRecordDomainModel medicalRecordModel = new MedicalRecordDomainModel 
        {
            Height = medicalRecord.Height,
            Weight = medicalRecord.Weight,
            BedriddenDiseases = medicalRecord.BedriddenDiseases,
            PatientId = medicalRecord.PatientId,
            IsDeleted = medicalRecord.IsDeleted
        };

        medicalRecordModel.AllergiesList = new List<AllergyDomainModel>();
        if (medicalRecord.AllergiesList != null)
        {
            foreach (Allergy item in medicalRecord.AllergiesList)
            {
                AllergyDomainModel allergy = new AllergyDomainModel
                {
                    IngredientId = item.IngredientId,
                    PatientId = item.PatientId
                };
                medicalRecordModel.AllergiesList.Add(allergy);
            }
        }


        medicalRecordModel.ReferralLetters = new List<ReferralLetterDomainModel>();
        if (medicalRecord.ReferralLetters != null)
        {
            foreach (ReferralLetter item in medicalRecord.ReferralLetters)
            {
                ReferralLetterDomainModel referralLetterModel = new ReferralLetterDomainModel
                {
                    Id = item.Id,
                    FromDoctorId = item.FromDoctorId,
                    ToDoctorId = item.ToDoctorId,
                    PatientId = item.PatientId
                };
                medicalRecordModel.ReferralLetters.Add(referralLetterModel);
            }

        }

        medicalRecordModel.Prescriptions = new List<PrescriptionDomainModel>();
        if (medicalRecord.Prescriptions != null)
        {
            foreach (Prescription item in medicalRecord.Prescriptions)
            {
                PrescriptionDomainModel prescription = new PrescriptionDomainModel
                {
                    Id = item.Id,
                    DrugId = item.DrugId,
                    PatientId = item.PatientId,
                    DoctorId = item.DoctorId,
                    TakeAt = item.TakeAt,
                    PerDay = item.PerDay,
                    IsDeleted = item.IsDeleted,
                    MealCombination = (MealCombination)Enum.Parse(typeof(MealCombination), item.MealCombination)
                };
                medicalRecordModel.Prescriptions.Add(prescription);
            }
        }
        return medicalRecordModel;
    }

    private MedicalRecord parseFromModel(MedicalRecordDomainModel medicalRecordModel)
    {
        MedicalRecord medicalRecord = new MedicalRecord 
        {
            Height = medicalRecordModel.Height,
            Weight = medicalRecordModel.Weight,
            BedriddenDiseases = medicalRecordModel.BedriddenDiseases,
            PatientId = medicalRecordModel.PatientId,
            IsDeleted = medicalRecordModel.IsDeleted
        };

        medicalRecord.AllergiesList = new List<Allergy>();
        if (medicalRecordModel.AllergiesList != null)
        {
            foreach (AllergyDomainModel item in medicalRecordModel.AllergiesList)
            {
                Allergy allergy = new Allergy
                {
                    IngredientId = item.IngredientId,
                    PatientId = item.PatientId
                };
                medicalRecord.AllergiesList.Add(allergy);
            }
        }

        medicalRecord.ReferralLetters = new List<ReferralLetter>();
        if (medicalRecordModel.ReferralLetters != null)
        {
            foreach(ReferralLetterDomainModel item in medicalRecordModel.ReferralLetters)
            {
                ReferralLetter referralLetter = new ReferralLetter
                {
                    Id = item.Id,
                    FromDoctorId = item.FromDoctorId,
                    ToDoctorId = item.ToDoctorId,
                    PatientId = item.PatientId
                };
                medicalRecord.ReferralLetters.Add(referralLetter);
            }

        }

        medicalRecord.Prescriptions = new List<Prescription>();
        if (medicalRecordModel.Prescriptions != null)
        {
            foreach(PrescriptionDomainModel item in medicalRecordModel.Prescriptions)
            {
                Prescription prescription = new Prescription
                {
                    Id = item.Id,
                    PatientId = item.PatientId,
                    DoctorId = item.DoctorId,
                    TakeAt = item.TakeAt,
                    PerDay = item.PerDay,
                    IsDeleted = item.IsDeleted,
                    MealCombination = item.MealCombination.ToString()
                };
                medicalRecord.Prescriptions.Add(prescription);
            }
        }
        return medicalRecord;
    }
}