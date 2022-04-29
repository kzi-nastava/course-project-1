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
            medicalRecordModel = new MedicalRecordDomainModel
            {
                IsDeleted = item.IsDeleted,
                Allergies = item.Allergies,
                BedriddenDiseases = item.BedriddenDiseases,
                Height = item.Height,
                PatientId = item.PatientId,
                Weight = item.Weight
            };
            results.Add(medicalRecordModel);
        }

        return results;
    } 

    public async Task<MedicalRecordDomainModel> GetForPatient(decimal id)
    {
        MedicalRecord data =  await _medicalRecordRepository.GetByPatientId(id);

        if (data == null) 
            throw new DataIsNullException();
        MedicalRecordDomainModel medicalRecordModel = new MedicalRecordDomainModel
        {
            IsDeleted = data.IsDeleted,
            Allergies = data.Allergies,
            BedriddenDiseases = data.BedriddenDiseases,
            Height = data.Height,
            PatientId = data.PatientId,
            Weight = data.Weight
        };
        return medicalRecordModel;
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
            Allergies = medicalRecord.Allergies,
            IsDeleted = medicalRecord.IsDeleted
        };
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
            Allergies = medicalRecordModel.Allergies,
            IsDeleted = medicalRecordModel.IsDeleted
        };
        return medicalRecord;
    }
}