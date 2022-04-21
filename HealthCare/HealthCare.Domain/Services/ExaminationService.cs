using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using HealthCare.Repositories;

namespace HealthCare.Domain.Services;

public class ExaminationService : IExaminationService{
    private IExaminationRepository _examinationRepository;

    public ExaminationService(IExaminationRepository examinationRepository) {
        _examinationRepository = examinationRepository;
    }

    // Async awaits info from database
    // GetAll is the equivalent of SELECT *
    public async Task<IEnumerable<ExaminationDomainModel>> GetAll()
    {
        var data = await _examinationRepository.GetAll();
        if (data == null)
            return null;

        List<ExaminationDomainModel> results = new List<ExaminationDomainModel>();
        ExaminationDomainModel examinationModel;
        foreach (var item in data)
        {
            examinationModel = new ExaminationDomainModel
            {
                StartTime = item.StartTime,
                Anamnesis = item.Anamnesis,
                doctorId = item.doctorId,
                Doctor = item.Doctor,
                ExaminationApproval = item.ExaminationApproval,
                IsDeleted = item.IsDeleted,
                patientId = item.patientId,
                Patient = item.Patient,
                roomId = item.roomId
            };
            results.Add(examinationModel);
        }

        return results;
    }
}