using HealthCare.Data.Entities;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using HealthCare.Repositories;

namespace HealthCare.Domain.Services;

public class ExaminationService : IExaminationService
{
    private IExaminationRepository _examinationRepository;
    private IExaminationApprovalRepository _examinationApprovalRepository;
    private IOperationRepository _operationRepository;
    private IRoomRepository _roomRepository;
    private IAntiTrollRepository _antiTrollRepository;
    private IAnamnesisRepository _anamnesisRepository;
    private IPatientRepository _patientRepository;

    public ExaminationService(IExaminationRepository examinationRepository,
                              IExaminationApprovalRepository examinationApprovalRepository,
                              IOperationRepository operationRepository,
                              IRoomRepository roomRepository,
                              IAntiTrollRepository antiTrollRepository,
                              IAnamnesisRepository anamnesisRepository,
                              IPatientRepository patientRepository)
    {
        _examinationRepository = examinationRepository;
        _examinationApprovalRepository = examinationApprovalRepository;
        _operationRepository = operationRepository;
        _roomRepository = roomRepository;
        _antiTrollRepository = antiTrollRepository;
        _anamnesisRepository = anamnesisRepository;
        _patientRepository = patientRepository;
    }

    private async Task<bool> AntiTrollCheck(decimal patientId, bool isCreate)
    {
        IEnumerable<AntiTroll> antiTrollHistory = await _antiTrollRepository.GetByPatientId(patientId);
        int createCounter = 0;
        int updateCounter = 0;
        foreach (AntiTroll item in antiTrollHistory)
        {
            double difference = (DateTime.Now - item.DateCreated).TotalDays;
            if (difference < 30)
            {
                if (item.State.Equals("create"))
                    createCounter++;
                else
                    updateCounter++;
            }
        }
        return isCreate ? createCounter > 8 : updateCounter > 5;
    }

    private ExaminationDomainModel parseToModel(Examination examination)
    {
        ExaminationDomainModel examinationModel = new ExaminationDomainModel
        {
            Id = examination.Id,
            StartTime = examination.StartTime,
            DoctorId = examination.DoctorId,
            IsDeleted = examination.IsDeleted,
            PatientId = examination.PatientId,
            RoomId = examination.RoomId
        };
        if (examination.Anamnesis != null)
        {
            examinationModel.Anamnesis = new AnamnesisDomainModel
            {
                Id = examination.Anamnesis.Id,
                Description = examination.Anamnesis.Description,
                ExaminationId = examination.Anamnesis.ExaminationId,
                IsDeleted = examination.Anamnesis.IsDeleted
            };
        }
        return examinationModel;
    }

    private Examination parseFromModel(ExaminationDomainModel examinationModel)
    {
        Examination examination = new Examination
        {
            Id = examinationModel.Id,
            StartTime = examinationModel.StartTime,
            DoctorId = examinationModel.DoctorId,
            IsDeleted = examinationModel.IsDeleted,
            PatientId = examinationModel.PatientId,
            RoomId = examinationModel.RoomId
        };
        if (examination.Anamnesis != null)
        {
            examination.Anamnesis = new Anamnesis
            {
                Id = examinationModel.Anamnesis.Id,
                Description = examinationModel.Anamnesis.Description,
                ExaminationId = examinationModel.Anamnesis.ExaminationId,
                IsDeleted = examinationModel.Anamnesis.IsDeleted
            };
        }
        return examination;
    }
    public async Task<IEnumerable<ExaminationDomainModel>> GetAll()
    {
        IEnumerable<Examination> data = await _examinationRepository.GetAll();
        if (data == null)
            return new List<ExaminationDomainModel>();

        List<ExaminationDomainModel> results = new List<ExaminationDomainModel>();
        foreach (Examination item in data)
        {
            results.Add(parseToModel(item));
        }

        return results;
    }

    public async Task<IEnumerable<ExaminationDomainModel>> ReadAll()
    {
        IEnumerable<ExaminationDomainModel> examinations = await GetAll();
        List<ExaminationDomainModel> result = new List<ExaminationDomainModel>();
        foreach (ExaminationDomainModel item in examinations)
        {
            if (!item.IsDeleted) result.Add(item);
        }
        return result;
    }

    public async Task<IEnumerable<ExaminationDomainModel>> GetAllForPatient(decimal id)
    {
        IEnumerable<Examination> data = await _examinationRepository.GetAllByPatientId(id);
        if (data == null)
            throw new DataIsNullException();

        List<ExaminationDomainModel> results = new List<ExaminationDomainModel>();
        foreach (Examination item in data)
        {
            results.Add(parseToModel(item));
        }

        return results;
    }

    public async Task<IEnumerable<ExaminationDomainModel>> GetAllForDoctor(decimal id)
    {
        IEnumerable<Examination> data = await _examinationRepository.GetAllByDoctorId(id);
        if (data == null)
            throw new DataIsNullException();

        List<ExaminationDomainModel> results = new List<ExaminationDomainModel>();
        foreach (Examination item in data)
        {
            results.Add(parseToModel(item));
        }

        return results;
    }

    public async Task<ExaminationDomainModel> Delete(ExaminationDomainModel examinationModel, bool isPatient)
    {
        if (isPatient && await AntiTrollCheck(examinationModel.PatientId, false))
            throw new DataIsNullException();
        Examination examination = await _examinationRepository.GetExamination(examinationModel.Id);
        double daysUntilExamination = (examination.StartTime - DateTime.Now).TotalDays;

        if (daysUntilExamination > 1 || !isPatient)
        {
            examination.IsDeleted = true;
            _ = _examinationRepository.Update(examination);
            _examinationRepository.Save();

            // anamnesis can't exist without its examination
            // check if anamnesis exists
            if (examination.Anamnesis != null)
            {
                examination.Anamnesis.IsDeleted = true;
                _ = _anamnesisRepository.Update(examination.Anamnesis);
                _anamnesisRepository.Save();
            }

        }
        else
        {
            ExaminationApproval examinationApproval = new ExaminationApproval
            {
                State = "created",
                IsDeleted = false,
                NewExaminationId = examination.Id,
                OldExaminationId = examination.Id
                //Examination = examination
            };
            _ = _examinationApprovalRepository.Post(examinationApproval);
            _examinationApprovalRepository.Save();
        }

        if (isPatient)
        {
            AntiTroll antiTrollItem = new AntiTroll
            {
                PatientId = examination.PatientId,
                State = "delete",
                DateCreated = DateTime.Now
            };

            _ = _antiTrollRepository.Post(antiTrollItem);
            _antiTrollRepository.Save();
        }
        return parseToModel(examination);
    }

    private async Task<bool> IsPatientOnExaminationAsync(ExaminationDomainModel examinationModel)
    {
        IEnumerable<Examination> patientsExaminations = await _examinationRepository.GetAllByPatientId(examinationModel.PatientId);
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

    private async Task<bool> IsPatientOnOperationAsync(ExaminationDomainModel examinationModel)
    {
        IEnumerable<Operation> patientsOperations = await _operationRepository.GetAllByPatientId(examinationModel.PatientId);
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

    private async Task<bool> IsDoctorOnExaminationAsync(ExaminationDomainModel examinationModel)
    {
        IEnumerable<Examination> doctorsExaminations = await _examinationRepository.GetAllByDoctorId(examinationModel.DoctorId);
        if (doctorsExaminations == null)
        {
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

    private async Task<bool> IsDoctorOnOperationAsync(ExaminationDomainModel examinationModel)
    {
        IEnumerable<Operation> doctorsOperations = await _operationRepository.GetAllByDoctorId(examinationModel.DoctorId);
        foreach (Operation operation in doctorsOperations)
        {
            double difference = (examinationModel.StartTime - operation.StartTime).TotalMinutes;
            if (difference <= (double)operation.Duration && difference >= -15)
            {
                return true;
            }
        }
        return false;
    }

    private async Task<decimal> GetAvailableRoomId(ExaminationDomainModel examinationModel)
    {
        IEnumerable<Room> rooms = await _roomRepository.GetAllAppointmentRooms("examination");
        foreach (Room room in rooms)
        {
            bool isRoomAvailable = true;
            IEnumerable<Examination> examinations = await _examinationRepository.GetAllByRoomId(room.Id);
            foreach (Examination examination in examinations)
            {
                double difference = (examinationModel.StartTime - examination.StartTime).TotalMinutes;
                if (difference <= 15 && difference >= -15)
                {
                    isRoomAvailable = false;
                    break;
                }
            }
            if (isRoomAvailable)
            {
                return room.Id;
            }
        }
        return -1;
    }


    public async Task<bool> IsDoctorAvailable(ExaminationDomainModel examinationModel)
    {
        return !(await IsDoctorOnExaminationAsync(examinationModel) ||
                 await IsDoctorOnOperationAsync(examinationModel));
    }

    public async Task<bool> IsPatientAvailable(ExaminationDomainModel examinationModel)
    {
        return !(await IsPatientOnExaminationAsync(examinationModel) ||
                 await IsPatientOnOperationAsync(examinationModel));
    }

    public async Task<ExaminationDomainModel> Create(ExaminationDomainModel examinationModel, bool isPatient)
    {
        if (isPatient && await AntiTrollCheck(examinationModel.PatientId, true))
            throw new AntiTrollException();
        if (examinationModel.StartTime <= DateTime.Now)
            throw new DateInPastExeption();
        bool doctorAvailable = await IsDoctorAvailable(examinationModel);
        bool patientAvailable = await IsPatientAvailable(examinationModel);
        if (!doctorAvailable)
            throw new DoctorNotAvailableException();
        if (!patientAvailable)
            throw new PatientNotAvailableException();

        decimal roomId = await GetAvailableRoomId(examinationModel);
        if (roomId == -1)
            throw new NoFreeRoomsException();


        Examination newExamination = new Examination 
        {
            PatientId = examinationModel.PatientId,
            RoomId = roomId,
            DoctorId = examinationModel.DoctorId,
            StartTime = removeSeconds(examinationModel.StartTime),
            IsDeleted = false,
            Anamnesis = null,
            //ExaminationApproval = null
        };

        if (isPatient) 
        {
            AntiTroll antiTrollItem = new AntiTroll 
            {
                PatientId = examinationModel.PatientId,
                State = "create",
                DateCreated = DateTime.Now
            };

            _ = _antiTrollRepository.Post(antiTrollItem);
            _antiTrollRepository.Save();
        }

        _ = _examinationRepository.Post(newExamination);
        _examinationRepository.Save();

        return examinationModel;
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

    public async Task<ExaminationDomainModel> Update(ExaminationDomainModel examinationModel, bool isPatient) 
    {
        // One patient can't change other patient's appointment
        // so the patient will always match examinationModel.PatientId
        if (isPatient && await AntiTrollCheck(examinationModel.PatientId, false))
            throw new AntiTrollException();
        Examination examination = await _examinationRepository.GetExaminationWithoutAnamnesis(examinationModel.Id);
        double daysUntilExamination = (examination.StartTime - DateTime.Now).TotalDays;
        if (examinationModel.StartTime <= DateTime.Now)
            throw new DateInPastExeption();
        bool doctorAvailable = await IsDoctorAvailable(examinationModel);
        bool patientAvailable = await IsPatientAvailable(examinationModel);
        if (!doctorAvailable)
            throw new DoctorNotAvailableException();
        if (!patientAvailable)
            throw new PatientNotAvailableException();

        decimal roomId = await GetAvailableRoomId(examinationModel);
        if (roomId == -1)
            throw new NoFreeRoomsException();



        if (daysUntilExamination > 1 || !isPatient) 
        { 
            
            examination.RoomId = roomId;
            examination.DoctorId = examinationModel.DoctorId;
            examination.PatientId = examinationModel.PatientId;
            examination.StartTime = removeSeconds(examinationModel.StartTime);
            //update
            _ = _examinationRepository.Update(examination);
            _examinationRepository.Save();

        } 
        else 
        {
            Examination newExamination = new Examination 
            {
                PatientId = examinationModel.PatientId,
                RoomId = roomId,
                DoctorId = examinationModel.DoctorId,
                StartTime = examinationModel.StartTime,
                IsDeleted = true,
                Anamnesis = null,
            };

            _ = _examinationRepository.Post(newExamination);
            _examinationRepository.Save();

            Examination createdExamination = await _examinationRepository.GetByParams(newExamination.DoctorId, newExamination.RoomId, newExamination.PatientId, newExamination.StartTime);

            // Make an approval request
            ExaminationApproval examinationApproval = new ExaminationApproval 
            {
                State = "created",
                IsDeleted = false,
                NewExaminationId = createdExamination.Id,
                OldExaminationId = examination.Id
                //Examination = examination
            };
            _ = _examinationApprovalRepository.Post(examinationApproval);
            _examinationApprovalRepository.Save();
        };
            

        if (isPatient) 
        {
            AntiTroll antiTrollItem = new AntiTroll 
            {
                PatientId = examinationModel.PatientId,
                State = "update",
                DateCreated = DateTime.Now
            };

            _ = _antiTrollRepository.Post(antiTrollItem);
            _antiTrollRepository.Save();
        }

        return examinationModel;
    }
}