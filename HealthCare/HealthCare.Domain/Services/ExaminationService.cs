using HealthCare.Data.Entities;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using HealthCare.Domain.Models.ModelsForUpdate;
using HealthCare.Repositories;

namespace HealthCare.Domain.Services;

public class ExaminationService : IExaminationService{
    private IExaminationRepository _examinationRepository;

    public ExaminationService(IExaminationRepository examinationRepository) {
        _examinationRepository = examinationRepository;
    }

    // Async awaits info from database
    // GetAll is the equivalent of SELECT *

    private ExaminationDomainModel parseToModel(Examination examination) {
        ExaminationDomainModel examinationModel = new ExaminationDomainModel {
            StartTime = examination.StartTime,
            doctorId = examination.doctorId,
            IsDeleted = examination.IsDeleted,
            patientId = examination.patientId,
            roomId = examination.roomId
        };
        if (examination.Anamnesis != null) {
            examinationModel.Anamnesis = new AnamnesisDomainModel {
                Description = examination.Anamnesis.Description,
                roomId = examination.Anamnesis.roomId,
                doctorId = examination.Anamnesis.doctorId,
                StartTime = examination.Anamnesis.StartTime,
                isDeleted = examination.Anamnesis.isDeleted
            };
        }
        return examinationModel;
    }
    public async Task<IEnumerable<ExaminationDomainModel>> GetAll()
    {
        var data = await _examinationRepository.GetAll();
        if (data == null)
            return null;

        List<ExaminationDomainModel> results = new List<ExaminationDomainModel>();
        
        foreach (var item in data)
        {           
            results.Add(parseToModel(item));
        }

        return results;
    }

    public async Task<IEnumerable<ExaminationDomainModel>> GetAllForPatient(decimal id) {
        var data = await _examinationRepository.GetAllByPatientId(id);
        if (data == null)
            return null;

        List<ExaminationDomainModel> results = new List<ExaminationDomainModel>();

        foreach (var item in data) {
            results.Add(parseToModel(item));
        }

        return results;
    }

    public async Task<ExaminationDomainModel> Delete(UpdateExaminationDomainModel updateExamination) {
        var examination = await _examinationRepository.GetExamination(updateExamination.roomId, updateExamination.doctorId, updateExamination.patientId, updateExamination.StartTime);
        var daysUntilExamination = (updateExamination.StartTime - examination.StartTime).TotalDays;
        if(daysUntilExamination > 1) {
            examination.IsDeleted = true;
            _ = _examinationRepository.Update(examination);
            _examinationRepository.Save();
        }
        return null;
       
    }
}