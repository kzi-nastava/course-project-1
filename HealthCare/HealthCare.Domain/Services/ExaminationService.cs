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
    private IAnamnesisRepository _anamnesisRepository;
    private IPatientRepository _patientRepository;

    public ExaminationService(IExaminationRepository examinationRepository, IExaminationApprovalRepository examinationApprovalRepository, IOperationRepository operationRepository, IRoomRepository roomRepository, IAntiTrollRepository antiTrollRepository, IAnamnesisRepository anamnesisRepository, IPatientRepository patientRepository)
    {
        _examinationRepository = examinationRepository;
        _examinationApprovalRepository = examinationApprovalRepository;
        _operationRepository = operationRepository;
        _roomRepository = roomRepository;
        _antiTrollRepository = antiTrollRepository;
        _anamnesisRepository = anamnesisRepository;
        _patientRepository = patientRepository;
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
            Id = examination.Id,
            StartTime = examination.StartTime,
            DoctorId = examination.DoctorId,
            IsDeleted = examination.IsDeleted,
            PatientId = examination.PatientId,
            RoomId = examination.RoomId
        };
        if (examination.Anamnesis != null) {
            examinationModel.Anamnesis = new AnamnesisDomainModel {
                Id = examination.Anamnesis.Id,
                Description = examination.Anamnesis.Description,
                ExaminationId = examination.Anamnesis.ExaminationId,
                IsDeleted = examination.Anamnesis.IsDeleted
            };
        }
        return examinationModel;
    }

    private Examination parseFromModel(ExaminationDomainModel examination) {
        Examination examinationModel = new Examination {
            Id = examination.Id,
            StartTime = examination.StartTime,
            DoctorId = examination.DoctorId,
            IsDeleted = examination.IsDeleted,
            PatientId = examination.PatientId,
            RoomId = examination.RoomId
        };
        if (examination.Anamnesis != null) {
            examinationModel.Anamnesis = new Anamnesis {
                Id = examination.Anamnesis.Id,
                Description = examination.Anamnesis.Description,
                ExaminationId = examination.Anamnesis.ExaminationId,
                IsDeleted = examination.Anamnesis.IsDeleted
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
    
    public async Task<IEnumerable<ExaminationDomainModel>> ReadAll()
    {
        IEnumerable<ExaminationDomainModel> examinations = await GetAll();
        List<ExaminationDomainModel> result = new List<ExaminationDomainModel>();
        foreach (var item in examinations)
        {           
            if (!item.IsDeleted) result.Add(item);
        }
        return result;
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

    public async Task<ExaminationDomainModel> Delete(DeleteExaminationDomainModel deleteExaminationModel, bool writeToAntiTroll) {
        if(deleteExaminationModel.IsPatient && await AntiTrollCheck(deleteExaminationModel.PatientId, false))
            return null;
        var examination = await _examinationRepository.GetExamination(deleteExaminationModel.ExaminationId);
        var daysUntilExamination = (examination.StartTime - DateTime.Now).TotalDays;
      
        if(daysUntilExamination > 1 || !deleteExaminationModel.IsPatient) {
            examination.IsDeleted = true;
            _ = _examinationRepository.Update(examination);
            _examinationRepository.Save();

            // anamnesis can't exist without its examination
            examination.Anamnesis.IsDeleted = true;
            _ = _anamnesisRepository.Update(examination.Anamnesis);
            _anamnesisRepository.Save();
            
        } else {
            ExaminationApproval examinationApproval = new ExaminationApproval {
                State = "created",
                IsDeleted = false,
                NewExaminationId = examination.Id,
                OldExaminationId = examination.Id
                //Examination = examination
            };
            _ = _examinationApprovalRepository.Post(examinationApproval);
            _examinationApprovalRepository.Save();
        }

        if (deleteExaminationModel.IsPatient && writeToAntiTroll) {
            AntiTroll antiTrollItem = new AntiTroll {
                PatientId = examination.PatientId,
                State = "delete",
                DateCreated = DateTime.Now
            };

            _ = _antiTrollRepository.Post(antiTrollItem);
            _antiTrollRepository.Save();
        }
        return parseToModel(examination);
       
    }

    private async Task<bool> IsPatientOnExaminationAsync(CreateExaminationDomainModel examinationModel)
    {
        var patientsExaminations = await _examinationRepository.GetAllByPatientId(examinationModel.PatientId);
        foreach (Examination examination in patientsExaminations)
        {
            if (examination.Id != examinationModel.Id)
            {
                double difference = (examinationModel.StartTime - examination.StartTime).TotalMinutes;
                if (difference <= 15 && difference >= -15)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private async Task<bool> IsPatientOnOperationAsync(CreateExaminationDomainModel examinationModel)
    {
        var patientsOperations = await _operationRepository.GetAllByPatientId(examinationModel.PatientId);
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
        var doctorsExaminations = await _examinationRepository.GetAllByDoctorId(examinationModel.DoctorId);
        if (doctorsExaminations == null) {
            return false;
        }
        foreach (Examination examination in doctorsExaminations) {
            if (examination.Id != examinationModel.Id)
            {
                double difference = (examinationModel.StartTime - examination.StartTime).TotalMinutes;
                if (difference <= 15 && difference >= -15)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private async Task<bool> IsDoctorOnOperationAsync(CreateExaminationDomainModel examinationModel) {
        var doctorsOperations = await _operationRepository.GetAllByDoctorId(examinationModel.DoctorId);
        foreach (Operation operation in doctorsOperations) {
            double difference = (examinationModel.StartTime - operation.StartTime).TotalMinutes;
            if (difference <= (double) operation.Duration && difference >= -15) {
                return true;
            }
        }
        return false;
    }

    private async Task<decimal> GetAvailableRoomId(CreateExaminationDomainModel examinationModel) {
        var rooms = await _roomRepository.GetAllAppointmentRooms("examination");
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

    public async Task<CreateExaminationDomainModel> Create(CreateExaminationDomainModel createExaminationModel, bool writeToAntiTroll) {
        if (createExaminationModel.IsPatient && await AntiTrollCheck(createExaminationModel.PatientId, true))
            return null;
        bool doctorAvailable = await IsDoctorAvailable(createExaminationModel);
        bool patientAvailable = await IsPatientAvailable(createExaminationModel);
        if (!doctorAvailable || !patientAvailable)
            //TODO: Think about the return value if doctor is not available
            return null;

        decimal roomId = await GetAvailableRoomId(createExaminationModel);
        if (roomId == -1) {
            return null;
        }

        int year = createExaminationModel.StartTime.Year;
        int month = createExaminationModel.StartTime.Month;
        int day = createExaminationModel.StartTime.Day;
        int hour = createExaminationModel.StartTime.Hour;
        int minute = createExaminationModel.StartTime.Minute;
        int second = 0;
        DateTime startTime = new DateTime(year, month, day, hour, minute, second);

        Examination newExamination = new Examination {
            PatientId = createExaminationModel.PatientId,
            RoomId = roomId,
            DoctorId = createExaminationModel.DoctorId,
            StartTime = startTime,
            IsDeleted = false,
            Anamnesis = null,
            //ExaminationApproval = null
        };

        if (createExaminationModel.IsPatient && writeToAntiTroll) {
            AntiTroll antiTrollItem = new AntiTroll {
                PatientId = createExaminationModel.PatientId,
                State = "create",
                DateCreated = DateTime.Now
            };

            _ = _antiTrollRepository.Post(antiTrollItem);
            _antiTrollRepository.Save();
        }

        _ = _examinationRepository.Post(newExamination);
        _examinationRepository.Save();

        /*
        Patient patient = await _patientRepository.GetPatientById(examinationModel.patientId);
        _ = _patientRepository.Update(patient);
        _patientRepository.Save();
        */

        return createExaminationModel;
    }

    public async Task<UpdateExaminationDomainModel> Update(UpdateExaminationDomainModel updateExaminationModel) {
        if (updateExaminationModel.isPatient && await AntiTrollCheck(updateExaminationModel.NewPatientId, false))
            return null;
        var examination = await _examinationRepository.GetExaminationWithoutAnamnesis(updateExaminationModel.OldExaminationId);
        var daysUntilExamination = (examination.StartTime - DateTime.Now).TotalDays;

        CreateExaminationDomainModel createExaminationDomainModel = new CreateExaminationDomainModel {
            Id = updateExaminationModel.OldExaminationId,
            DoctorId = updateExaminationModel.NewDoctorId,
            PatientId = updateExaminationModel.NewPatientId,
            StartTime = updateExaminationModel.NewStartTime,
            IsPatient = updateExaminationModel.isPatient
        };
        bool doctorAvailable = await IsDoctorAvailable(createExaminationDomainModel);
        bool patientAvailable = await IsPatientAvailable(createExaminationDomainModel);
        if (!doctorAvailable || !patientAvailable)
            //TODO: Think about the return value if doctor is not available
            return null;

        decimal roomId = await GetAvailableRoomId(createExaminationDomainModel);
        if (roomId == -1) {
            return null;
        }

        if (daysUntilExamination > 1 || !updateExaminationModel.isPatient) { 
            
            examination.RoomId = roomId;
            examination.DoctorId = updateExaminationModel.NewDoctorId;
            examination.PatientId = updateExaminationModel.NewPatientId;
            examination.StartTime = updateExaminationModel.NewStartTime;
            //update
            Examination updatedExamination = _examinationRepository.Update(examination);
            _examinationRepository.Save();

        } else {
            Examination newExamination = new Examination {
                PatientId = updateExaminationModel.NewPatientId,
                RoomId = roomId,
                DoctorId = updateExaminationModel.NewDoctorId,
                StartTime = updateExaminationModel.NewStartTime,
                IsDeleted = true,
                Anamnesis = null,
            };

            _ = _examinationRepository.Post(newExamination);
            _examinationRepository.Save();

            var createdExamination = await _examinationRepository.GetByParams(newExamination.DoctorId, newExamination.RoomId, newExamination.PatientId, newExamination.StartTime);

            // Make an approval request
            ExaminationApproval examinationApproval = new ExaminationApproval {
                State = "created",
                IsDeleted = false,
                NewExaminationId = createdExamination.Id,
                OldExaminationId = examination.Id
                //Examination = examination
            };
            _ = _examinationApprovalRepository.Post(examinationApproval);
            _examinationApprovalRepository.Save();
        };
            

        if (updateExaminationModel.isPatient) {
            AntiTroll antiTrollItem = new AntiTroll {
                PatientId = updateExaminationModel.NewPatientId,
                State = "update",
                DateCreated = DateTime.Now
            };

            _ = _antiTrollRepository.Post(antiTrollItem);
            _antiTrollRepository.Save();
        }

        return updateExaminationModel;
    }
}