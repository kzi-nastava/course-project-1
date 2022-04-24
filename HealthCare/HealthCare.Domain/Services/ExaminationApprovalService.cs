using HealthCare.Data.Entities;
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
                Id = item.Id,
                isDeleted = item.isDeleted,
                OldDoctorId = item.OldDoctorId,
                OldPatientId = item.OldPatientId,
                OldRoomId = item.OldRoomId,
                OldStartTime = item.OldStartTime,
                NewDoctorId = item.NewDoctorId,
                NewPatientId = item.NewPatientId,
                NewRoomId = item.NewRoomId,
                NewStartTime = item.NewStartTime,
                State = item.State
            };
            //if (item.Examination != null)
            //    examinationApprovalModel.Examination = new ExaminationDomainModel {
            //        doctorId = item.Examination.doctorId,
            //        roomId = item.Examination.roomId,
            //        patientId = item.Examination.patientId,
            //        StartTime = item.Examination.StartTime,
            //        IsDeleted = item.Examination.IsDeleted
            //    };
            results.Add(examinationApprovalModel);
        }

        return results;
    }

    public async Task<ExaminationApprovalDomainModel> Reject(ExaminationApprovalDomainModel examinationModel)
    {
        if (!examinationModel.State.Equals("created")) return null;
        ExaminationApproval examinationApproval = await _examinationApprovalRepository.GetExaminationApprovalById(examinationModel.Id);
        examinationApproval.State = "rejected";
        _ = _examinationApprovalRepository.Update(examinationApproval);
        _examinationApprovalRepository.Save();
        examinationModel.State = "rejected";
        return examinationModel;
    }

    public async Task<ExaminationApprovalDomainModel> Approve(ExaminationApprovalDomainModel examinationModel)
    {
        throw new NotImplementedException();
    }
}