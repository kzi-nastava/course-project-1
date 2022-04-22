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
        
        foreach (var item in data)
        {
            ExaminationDomainModel examinationModel = new ExaminationDomainModel {
                StartTime = item.StartTime,
                doctorId = item.doctorId,
                //Doctor = item.Doctor,
                IsDeleted = item.IsDeleted,
                patientId = item.patientId,
                //Patient = item.Patient,
                roomId = item.roomId
            };
            if (item.Anamnesis != null) {
                examinationModel.Anamnesis = new AnamnesisDomainModel {
                    Description = item.Anamnesis.Description,
                    roomId = item.Anamnesis.roomId,
                    doctorId = item.Anamnesis.doctorId,
                    StartTime = item.Anamnesis.StartTime,
                    isDeleted = item.Anamnesis.isDeleted
                };
            }
            //if (item.ExaminationApproval != null) {
            //    examinationModel.ExaminationApproval = new ExaminationApprovalDomainModel {
            //        State = item.ExaminationApproval.State,
            //        RoomId = item.ExaminationApproval.RoomId,
            //        DoctorId = item.ExaminationApproval.DoctorId,
            //        StartTime = item.ExaminationApproval.StartTime,
            //        PatientId = item.ExaminationApproval.PatientId,
            //        isDeleted = item.ExaminationApproval.isDeleted
            //    };
            //}
                
            results.Add(examinationModel);
        }

        return results;
    }
}