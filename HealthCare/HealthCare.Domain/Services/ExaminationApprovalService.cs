using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using HealthCare.Repositories;

namespace HealthCare.Domain.Services;

public class ExaminationApprovalService : IExaminationApprovalService{
    private IExaminationApprovalRepository _examinationApprovalRepository;

    public ExaminationApprovalService(IExaminationApprovalRepository examinationApprovalRepository) {
        _examinationApprovalRepository = examinationApprovalRepository;
    }

    // Async awaits info from database
    // GetAll is the equivalent of SELECT *
    public async Task<IEnumerable<ExaminationApprovalDomainModel>> GetAll()
    {
        var data = await _examinationApprovalRepository.GetAll();
        if (data == null)
            return null;

        List<ExaminationApprovalDomainModel> results = new List<ExaminationApprovalDomainModel>();
        ExaminationApprovalDomainModel examinationApprovalModel;
        foreach (var item in data)
        {
            examinationApprovalModel = new ExaminationApprovalDomainModel
            {
                isDeleted = item.isDeleted,
                DoctorId = item.DoctorId,
                PatientId = item.PatientId,
                RoomId = item.RoomId,
                StartTime = item.StartTime,
                State = item.State
            };
            if (item.Examination != null)
                examinationApprovalModel.Examination = new ExaminationDomainModel {
                    doctorId = item.Examination.doctorId,
                    roomId = item.Examination.roomId,
                    patientId = item.Examination.patientId,
                    StartTime = item.Examination.StartTime,
                    IsDeleted = item.Examination.IsDeleted
                };
            results.Add(examinationApprovalModel);
        }

        return results;
    }    
}