using HealthCare.Data.Entities;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using HealthCare.Domain.Models.ModelsForCreate;
using HealthCare.Domain.Models.ModelsForUpdate;
using HealthCare.Repositories;

namespace HealthCare.Domain.Services;

public class ExaminationService : IExaminationService{
    private IExaminationRepository _examinationRepository;
    private IExaminationApprovalRepository _examinationApprovalRepository;
    private IOperationRepository _operationRepository;
    private IRoomRepository _roomRepository;
    private IAntiTrollRepository _antiTrollRepository;

    public ExaminationService(IExaminationRepository examinationRepository, IExaminationApprovalRepository examinationApprovalRepository, IOperationRepository operationRepository, IRoomRepository roomRepository, IAntiTrollRepository antiTrollRepository) {
        _examinationRepository = examinationRepository;
        _examinationApprovalRepository = examinationApprovalRepository;
        _operationRepository = operationRepository;
        _roomRepository = roomRepository;
        _antiTrollRepository = antiTrollRepository;
    }

    // Async awaits info from database
    // GetAll is the equivalent of SELECT *


    private async Task<bool> AntiTrollCheck(decimal patientId, bool isCreate) {
        var antiTrollHistory = await _antiTrollRepository.GetByPatientId(patientId);
        int createCounter = 0;
        int updateCounter = 0;
        foreach (var item in antiTrollHistory) {
            double difference = (DateTime.Now - item.DateCreated).TotalDays;
            if (difference < 30) {
                if (item.State.Equals("create"))
                    createCounter++;
                else
                    updateCounter++;
            }
        }
        return isCreate ? createCounter > 8 : updateCounter > 5;
    }

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

    public async Task<IEnumerable<ExaminationDomainModel>> GetAllForDoctor(decimal id)
    {
        var data = await _examinationRepository.GetAllByDoctorId(id);
        if (data == null)
            return null;

        List<ExaminationDomainModel> results = new List<ExaminationDomainModel>();

        foreach (var item in data)
        {
            results.Add(parseToModel(item));
        }

        return results;
    }

    public async Task<ExaminationDomainModel> Delete(DeleteExaminationDomainModel deleteExamination) {
        if(await AntiTrollCheck(deleteExamination.patientId, false))
            return null;
        var examination = await _examinationRepository.GetExaminationWithoutAnamnesis(deleteExamination.roomId, deleteExamination.doctorId, deleteExamination.patientId, deleteExamination.StartTime);
        var daysUntilExamination = (deleteExamination.StartTime - DateTime.Now).TotalDays;
      
        if(daysUntilExamination > 1) {
            examination.IsDeleted = true;
            _ = _examinationRepository.Update(examination);
            _examinationRepository.Save();
        } else {
            ExaminationApproval examinationApproval = new ExaminationApproval {
                State = "created",
                OldDoctorId = examination.doctorId,
                OldPatientId = examination.patientId,
                OldRoomId = examination.roomId,
                OldStartTime = examination.StartTime,
                NewDoctorId = examination.doctorId,
                NewPatientId = examination.patientId,
                NewRoomId = examination.roomId,
                NewStartTime = examination.StartTime,
                isDeleted = false,
                //Examination = examination
            };
            _ = _examinationApprovalRepository.Post(examinationApproval);
            _examinationApprovalRepository.Save();
        }

        AntiTroll antiTrollItem = new AntiTroll {
            PatientId = examination.patientId,
            State = "delete",
            DateCreated = DateTime.Now
        };

        _ = _antiTrollRepository.Post(antiTrollItem);
        _antiTrollRepository.Save();
        return null;
       
    }

    private async Task<bool> IsPatientOnExaminationAsync(CreateExaminationDomainModel examinationModel)
    {
        var patientsExaminations = await _examinationRepository.GetAllByPatientId(examinationModel.patientId);
        foreach (Examination examination in patientsExaminations)
        {
            double difference = (examinationModel.StartTime - examination.StartTime).TotalMinutes;
            if (difference <= 15 && difference >= -15)
            {
                return true;
            }
        }
        return false;
    }

    private async Task<bool> IsPatientOnOperationAsync(CreateExaminationDomainModel examinationModel)
    {
        var patientsOperations = await _operationRepository.GetAllByPatientId(examinationModel.patientId);
        foreach (Operation operation in patientsOperations)
        {
            double difference = (examinationModel.StartTime - operation.StartTime).TotalMinutes;
            if (difference <= (double)operation.Duration && difference >= -15)
            {
                return true;
            }
        }
        return false;
    }

    private async Task<bool> IsDoctorOnExaminationAsync(CreateExaminationDomainModel examinationModel) {
        var doctorsExaminations = await _examinationRepository.GetAllByDoctorId(examinationModel.doctorId);
        if (doctorsExaminations == null) {
            return false;
        }
        foreach (Examination examination in doctorsExaminations) {
            double difference = (examinationModel.StartTime - examination.StartTime).TotalMinutes;
            if (difference <= 15 && difference >= -15) {
                return true;
            }
        }
        return false;
    }

    private async Task<bool> IsDoctorOnOperationAsync(CreateExaminationDomainModel examinationModel) {
        var doctorsOperations = await _operationRepository.GetAllByDoctorId(examinationModel.doctorId);
        foreach (Operation operation in doctorsOperations) {
            double difference = (examinationModel.StartTime - operation.StartTime).TotalMinutes;
            if (difference <= (double) operation.Duration && difference >= -15) {
                return true;
            }
        }
        return false;
    }

    private async Task<decimal> GetAvailableRoomId(CreateExaminationDomainModel examinationModel) {
        var rooms = await _roomRepository.GetAllExaminationRooms();
        foreach (Room room in rooms) {
            bool isRoomAvailable = true;
            var examinations = await _examinationRepository.GetAllByRoomId(room.Id);
            foreach (Examination examination in examinations) {
                double difference = (examinationModel.StartTime - examination.StartTime).TotalMinutes;
                if (difference <= 15 && difference >= -15) {
                    isRoomAvailable = false;
                    break;
                }
            }
            if (isRoomAvailable) {
                return room.Id;
            }
        }
        return -1;
    }


    public async Task<bool> IsDoctorAvailable(CreateExaminationDomainModel examinationModel)
    {
        bool isDoctorAvailable = true;
        if (await IsDoctorOnExaminationAsync(examinationModel))
            isDoctorAvailable = false;
        if (await IsDoctorOnOperationAsync(examinationModel))
            isDoctorAvailable = false;

        if (!isDoctorAvailable)
        {
            //TODO: Think about the return value if doctor is not available
            return false;
        }

        return true;
    }

    public async Task<bool> IsPatientAvailable(CreateExaminationDomainModel examinationModel)
    {
        bool isPatientAvailable = true;
        if (await IsPatientOnExaminationAsync(examinationModel))
            isPatientAvailable = false;
        if (await IsPatientOnOperationAsync(examinationModel))
            isPatientAvailable = false;

        if (!isPatientAvailable)
        {
            return false;
        }

        return true;
    }

    public async Task<CreateExaminationDomainModel> Create(CreateExaminationDomainModel examinationModel) {
        if (await AntiTrollCheck(examinationModel.patientId, true))
            return null;
        bool doctorAvailable = await IsDoctorAvailable(examinationModel);
        bool patientAvailable = await IsPatientAvailable(examinationModel);
        if (!doctorAvailable || !patientAvailable)
            //TODO: Think about the return value if doctor is not available
            return null;

        decimal roomId = await GetAvailableRoomId(examinationModel);
        if (roomId == -1) {
            return null;
        }

        Examination newExamination = new Examination {
            patientId = examinationModel.patientId,
            roomId = roomId,
            doctorId = examinationModel.doctorId,
            StartTime = examinationModel.StartTime,
            IsDeleted = false,
            Anamnesis = null,
            //ExaminationApproval = null
        };

        AntiTroll antiTrollItem = new AntiTroll {
            PatientId = examinationModel.patientId,
            State = "create",
            DateCreated = DateTime.Now
        };

        _ = _antiTrollRepository.Post(antiTrollItem);
        _antiTrollRepository.Save();


        _ = _examinationRepository.Post(newExamination);
        _examinationRepository.Save();

        return examinationModel;
    }

    public async Task<UpdateExaminationDomainModel> Update(UpdateExaminationDomainModel examinationModel) {
        if (await AntiTrollCheck(examinationModel.oldPatientId, false))
            return null;
        var examination = await _examinationRepository.GetExaminationWithoutAnamnesis(examinationModel.oldRoomId, examinationModel.oldDoctorId, examinationModel.oldPatientId, examinationModel.oldStartTime);
        var daysUntilExamination = (examinationModel.oldStartTime - DateTime.Now).TotalDays;
        
        if(daysUntilExamination > 1) {
            // Delete existing
            examination.IsDeleted = true; 
            _ = _examinationRepository.Update(examination);
            _examinationRepository.Save();
            // Create new
            Examination newExamination = new Examination {
                patientId = examinationModel.newPatientId,
                roomId = examinationModel.newRoomId,
                doctorId = examinationModel.newDoctorId,
                StartTime = examinationModel.newStartTime,
                IsDeleted = false,
                Anamnesis = null,
            };
            _ = _examinationRepository.Post(newExamination);
            _examinationRepository.Save();

        } else {
            // Make an approval request
            ExaminationApproval examinationApproval = new ExaminationApproval {
                State = "created",
                OldDoctorId = examinationModel.oldDoctorId,
                OldPatientId = examinationModel.oldPatientId,
                OldRoomId = examinationModel.oldRoomId,
                OldStartTime = examinationModel.oldStartTime,
                NewDoctorId = examinationModel.newDoctorId,
                NewPatientId = examinationModel.newPatientId,
                NewRoomId = examinationModel.newRoomId,
                NewStartTime = examinationModel.newStartTime,
                isDeleted = false
            };
            _ = _examinationApprovalRepository.Post(examinationApproval);
            _examinationApprovalRepository.Save();
        }

        AntiTroll antiTrollItem = new AntiTroll {
            PatientId = examinationModel.newPatientId,
            State = "update",
            DateCreated = DateTime.Now
        };

        _ = _antiTrollRepository.Post(antiTrollItem);
        _antiTrollRepository.Save();

        return examinationModel;
    }
}