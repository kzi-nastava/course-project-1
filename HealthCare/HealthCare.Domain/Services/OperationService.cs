using HealthCare.Data.Entities;
using HealthCare.Domain.DTOs;
using HealthCare.Domain.Models;
using HealthCare.Repositories;
using Microsoft.AspNetCore.Mvc.Razor.TagHelpers;
using Microsoft.OpenApi.Any;

namespace HealthCare.Domain.Interfaces;

public class OperationService : IOperationService 
{
    private IOperationRepository _operationRepository;
    private IRoomRepository _roomRepository;
    private IExaminationRepository _examinationRepository;
    private IPatientRepository _patientRepository;
    private IDoctorRepository _doctorRepository;

    public OperationService(IOperationRepository operationRepository, 
                            IRoomRepository roomRepository, 
                            IExaminationRepository examinationRepository,
                            IPatientRepository patientRepository,
                            IDoctorRepository doctorRepository) 
    {
        _operationRepository = operationRepository;
        _roomRepository = roomRepository;
        _examinationRepository = examinationRepository;
        _patientRepository = patientRepository;
        _doctorRepository = doctorRepository;
    }

    public async Task<IEnumerable<OperationDomainModel>> ReadAll()
    {
        IEnumerable<OperationDomainModel> operations = await GetAll();
        List<OperationDomainModel> result = new List<OperationDomainModel>();
        foreach (OperationDomainModel item in operations)
        {
            if (!item.IsDeleted) result.Add(item);
        }
        return result;
    }
    public async Task<IEnumerable<OperationDomainModel>> GetAll()
    {
        IEnumerable<Operation> data = await _operationRepository.GetAll();
        if (data == null)
            return new List<OperationDomainModel>();
        List<OperationDomainModel> results = new List<OperationDomainModel>();
        OperationDomainModel operationModel;
        foreach (Operation item in data)
        {
            results.Add(ParseToModel(item));
        }

        return results;
    }

    public async Task<IEnumerable<OperationDomainModel>> GetAllForDoctor(decimal id)
    {
        IEnumerable<Operation> data = await _operationRepository.GetAllByDoctorId(id);
        if (data == null)
            throw new DataIsNullException();

        List<OperationDomainModel> results = new List<OperationDomainModel>();
        foreach (Operation item in data)
        {
            results.Add(ParseToModel(item));
        }

        return results;
    }


    private async Task<decimal> GetAvailableRoomId(DateTime startTime, decimal duration)
    {
        IEnumerable<Room> rooms = await _roomRepository.GetAllAppointmentRooms("operation");
        foreach (Room room in rooms)
        {
            bool isRoomAvailable = true;
            IEnumerable<Operation> operations = await _operationRepository.GetAllByRoomId(room.Id);
            foreach (Operation operation in operations)
            {
                double difference = (startTime - operation.StartTime).TotalMinutes;
                if (difference <= (double)operation.Duration && difference >= -(double)duration)
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

    private async Task<bool> isPatientOnExamination(CUOperationDTO dto)
    {
        IEnumerable<Examination> patientsExaminations = await _examinationRepository.GetAllByPatientId(dto.PatientId);
        foreach (Examination examination in patientsExaminations)
        {
            double difference = (dto.StartTime - examination.StartTime).TotalMinutes;
            if (difference <= 15 && difference >= -(double)dto.Duration)
            {
                return true;
            }
        }
        return false;
    }

    private async Task<bool> isPatientOnOperation(CUOperationDTO dto)
    {
        IEnumerable<Operation> patientsOperations = await _operationRepository.GetAllByPatientId(dto.PatientId);
        foreach (Operation operation in patientsOperations)
        {
            if (operation.Id != dto.Id)
            {
                double difference = (dto.StartTime - operation.StartTime).TotalMinutes;
                if (difference <= (double)operation.Duration && difference >= -(double)dto.Duration)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private async Task<bool> isDoctorOnExamination(CUOperationDTO dto)
    {
        IEnumerable<Examination> doctorsExaminations = await _examinationRepository.GetAllByDoctorId(dto.DoctorId);
        if (doctorsExaminations == null)
            return false;
        
        foreach (Examination examination in doctorsExaminations)
        {
            double difference = (dto.StartTime - examination.StartTime).TotalMinutes;
            if (difference <= 15 && difference >= -(double)dto.Duration)
            {
                return true;
            }
        }
        return false;
    }

    private async Task<bool> isDoctorOnOperation(CUOperationDTO dto)
    {
        IEnumerable<Operation> doctorsOperations = await _operationRepository.GetAllByDoctorId(dto.DoctorId);
        foreach (Operation operation in doctorsOperations)
        {
            if (operation.Id != dto.Id)
            {
                double difference = (dto.StartTime - operation.StartTime).TotalMinutes;
                if (difference <= (double)operation.Duration && difference >= -(double)dto.Duration)
                {
                    return true;
                }
            }
           
        }
        return false;
    }

    private async Task<bool> isDoctorAvailable(CUOperationDTO dto)
    {
        return !(await isDoctorOnOperation(dto) ||
                 await isDoctorOnExamination(dto));
    }

    private async Task<bool> isPatientAvailable(CUOperationDTO dto)
    {
        return !(await isPatientOnOperation(dto) ||
                 await isPatientOnExamination(dto));
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

    private async Task<bool> isPatientBlocked(decimal patientId)
    {
        Patient patient = await _patientRepository.GetPatientById(patientId);
        if (patient.BlockedBy != null && !patient.BlockedBy.Equals(""))
            return true;

        return false;
    }

    private async Task validateUserInput(CUOperationDTO dto)
    {
        if (dto.StartTime <= DateTime.UtcNow)
            throw new DateInPastExeption();
        if (await isPatientBlocked(dto.PatientId))
            throw new PatientIsBlockedException();
        bool doctorAvailable = await isDoctorAvailable(dto);
        bool patientAvailable = await isPatientAvailable(dto);
        if (!doctorAvailable)
            throw new DoctorNotAvailableException();
        if (!patientAvailable)
            throw new PatientNotAvailableException();
    }

    public async Task<OperationDomainModel> Create(CUOperationDTO dto)
    {
        await validateUserInput(dto);

        decimal roomId = await GetAvailableRoomId(dto.StartTime, dto.Duration);
        if (roomId == -1)
            throw new NoFreeRoomsException();

        Operation newOperation = new Operation
        {
            PatientId = dto.PatientId,
            RoomId = roomId,
            DoctorId = dto.DoctorId,
            StartTime = removeSeconds(dto.StartTime),
            Duration = dto.Duration,
            IsDeleted = false
        };

        _ = _operationRepository.Post(newOperation);
        _operationRepository.Save();

        return ParseToModel(newOperation);
    }

    public async Task<OperationDomainModel> Update(CUOperationDTO dto)
    {
        await validateUserInput(dto);

        Operation operation = await _operationRepository.GetById(dto.Id);

        if (operation == null)
            throw new DataIsNullException();

        decimal roomId = await GetAvailableRoomId(dto.StartTime, dto.Duration);
        if (roomId == -1)
            throw new NoFreeRoomsException();

        operation.PatientId = dto.PatientId;
        operation.DoctorId = dto.DoctorId;
        operation.Duration = dto.Duration;
        operation.StartTime = removeSeconds(dto.StartTime);

        _ = _operationRepository.Update(operation);
        _operationRepository.Save();

        return ParseToModel(operation);
    }

    public async Task<OperationDomainModel> Delete(decimal id)
    {
        Operation operation = await _operationRepository.GetById(id);
        
        // logical delete
        operation.IsDeleted = true;
        _ = _operationRepository.Update(operation);
        _operationRepository.Save();

        return ParseToModel(operation);
    }

    public static OperationDomainModel ParseToModel(Operation operation)
    {
        OperationDomainModel operationModel = new OperationDomainModel
        {
            Id = operation.Id,
            StartTime = operation.StartTime,
            Duration = operation.Duration,
            RoomId = operation.RoomId,
            DoctorId = operation.DoctorId,
            PatientId = operation.PatientId,
            IsDeleted = operation.IsDeleted,
            IsEmergency = operation.IsEmergency
        };

        return operationModel;
    }
    
    public static Operation ParseFromModel(OperationDomainModel operationModel)
    {
        Operation operation = new Operation
        {
            Id = operationModel.Id,
            StartTime = operationModel.StartTime,
            Duration = operationModel.Duration,
            RoomId = operationModel.RoomId,
            DoctorId = operationModel.DoctorId,
            PatientId = operationModel.PatientId,
            IsDeleted = operationModel.IsDeleted,
            IsEmergency = operationModel.IsEmergency
        };

        return operation;
    }
    
    public async Task<DateTime?> FirstStartTime(decimal doctorId, List<KeyValuePair<DateTime, DateTime>> schedule, DateTime now)
    {
        DateTime limit = removeSeconds(now.AddHours(2));
        foreach (KeyValuePair<DateTime, DateTime> pair in schedule)
        {
            // Now: 20:00, Limit: 22:00, Schedule: 14:00 - 16:00 -> continue
            if (now > pair.Value) continue;
            // Now: 20:00, Limit: 22:00, Schedule: 15:00 - 21:00 -> 20:00
            if (now >= pair.Key && now <= pair.Value) return now;
            // Now: 20:00, Limit: 22:00, Schedule: 21:00 - 23:00 -> 21:00
            if (limit >= pair.Key && pair.Key > now) return pair.Key;
            // Now: 20:00, Limit: 22:00, Schedule: 23:00 - 23:30 -> break completely (every other
            // pair will be greater than this one, so return null)
            return null;
        }
        return null;
    }

    public async Task<IEnumerable<OperationDomainModel>> CreateUrgent(decimal patientId, decimal specializationId, decimal duration, IDoctorService doctorService, IPatientService patientService)
    {
        DateTime now = removeSeconds(DateTime.Now);
        OperationDomainModel operationModel = new OperationDomainModel
        {
            IsDeleted = false,
            PatientId = patientId,
            Duration = duration,
            IsEmergency = true
        };
        // Find examination in the first 2 hours for any doctor that matches
        // the specialization criteria
        List<Doctor> doctors = (List<Doctor>)await _doctorRepository.GetBySpecialization(specializationId);
        if (doctors == null || doctors.Count == 0) throw new NoAvailableSpecialistsException();
        List<KeyValuePair<DateTime, decimal>> urgentStartTimes = new List<KeyValuePair<DateTime, decimal>>();
        foreach (Doctor doctor in doctors)
        {
            var schedule =
                (List<KeyValuePair<DateTime, DateTime>>)await doctorService.GetAvailableSchedule(doctor.Id, duration);
            DateTime? startTime = await FirstStartTime(doctor.Id, schedule, now);
            if (startTime.HasValue)
                urgentStartTimes.Add(new KeyValuePair<DateTime, decimal>(startTime.GetValueOrDefault(), doctor.Id));
        }

        urgentStartTimes.Sort((x, y) => x.Key.CompareTo(y.Key));
        // Try to create examination
        foreach (KeyValuePair<DateTime, decimal> pair in urgentStartTimes)
        {
            operationModel.StartTime = pair.Key;
            operationModel.DoctorId = pair.Value;
            decimal roomId = await GetAvailableRoomId(operationModel.StartTime, operationModel.Duration);
            if (roomId == -1) continue;
            operationModel.RoomId = roomId;
            Operation operation = ParseFromModel(operationModel);
            _ = _operationRepository.Post(operation);
            _operationRepository.Save();
            // Return empty list to signify success
            return new List<OperationDomainModel>();
        }

        // Above failed, return examinations that can be postponed
        // sorted by the date on which they can be postponed 
        // This list must contain 5 examinations
        // TODO: Dto candidate
        Dictionary<decimal, KeyValuePair<OperationDomainModel, DateTime>> canBeRescheduled =
            new Dictionary<decimal, KeyValuePair<OperationDomainModel, DateTime>>();
        foreach (Doctor doctor in doctors)
        {
            // Available doctor schedule
            var availableSchedule =
                (List<KeyValuePair<DateTime, DateTime>>)await doctorService.GetAvailableSchedule(doctor.Id);
            // Busy doctor schedule
            var busySchedule =
                (List<KeyValuePair<DateTime, DateTime>>)await doctorService.GetBusySchedule(doctor.Id);
            // Patient schedule
            var patientSchedule = 
                (List<KeyValuePair<DateTime, DateTime>>)await patientService.GetSchedule(patientId);
            var first = await GetFirstForReschedule(busySchedule, availableSchedule, patientSchedule, doctor.Id,
                patientId, duration);
            if (first.Key == null) continue;
            canBeRescheduled.Add(doctor.Id, first); 
        }
        var sortedDict = 
            from entry in canBeRescheduled orderby entry.Value.Value select entry;
        List<OperationDomainModel> result = new List<OperationDomainModel>();
        int counter = 0;
        foreach (var entry in sortedDict)
        {
            result.Add(entry.Value.Key);
            counter++;
            if (counter == 5) break;
        }

        return result;
    }

    public async Task<Boolean> IsUrgent(Operation operation)
    {
        return operation.IsEmergency;
    }

    public async Task<KeyValuePair<OperationDomainModel, DateTime>> GetFirstForReschedule(List<KeyValuePair<DateTime, DateTime>> busySchedule, 
        List<KeyValuePair<DateTime, DateTime>> availableSchedule, List<KeyValuePair<DateTime, DateTime>> patientSchedule, decimal doctorId, decimal patientId, decimal duration)
    {
        DateTime now = removeSeconds(DateTime.Now);
        DateTime limit = removeSeconds(now.AddHours(2));
        CUOperationDTO mockupModel = new CUOperationDTO
        {
            StartTime = now,
            Duration = duration
        };
        OperationDomainModel? operationModel = null;
        foreach (KeyValuePair<DateTime, DateTime> pair in busySchedule)
        {
            // If limit is larger than pair key then we cannot reschedule
            if (now > pair.Value) continue;
            if (limit > pair.Key) break;
            // Rescheduling ahead
            if (mockupModel.StartTime < pair.Key && mockupModel.StartTime.AddMinutes((double)duration) >= pair.Key &&
                mockupModel.StartTime.AddMinutes((double)duration) <= pair.Value && await isDoctorAvailable(mockupModel))
            {
                // If it's urgent, then skip it
                if (await IsUrgent(await _operationRepository.GetByDoctorPatientDate(doctorId, patientId, pair.Key)))
                    continue;
                // Find this operation
                Operation operation = await _operationRepository.GetByDoctorPatientDate(doctorId, patientId, pair.Key);
                operationModel = ParseToModel(operation);
            }

            mockupModel.StartTime.AddMinutes((double)duration);
            if (mockupModel.StartTime > limit) break;
            // Rescheduling behind
            if (mockupModel.StartTime > pair.Value && mockupModel.StartTime.AddMinutes((double)-duration) > pair.Key
               && mockupModel.StartTime.AddMinutes((double)-duration) < pair.Value && await isDoctorAvailable(mockupModel))
            {
                // If it's urgent, then skip it
                if (await IsUrgent(await _operationRepository.GetByDoctorPatientDate(doctorId, patientId, pair.Key)))
                    continue;
                // Find this operation
                Operation operation = await _operationRepository.GetByDoctorPatientDate(doctorId, patientId, pair.Key);
                operationModel = ParseToModel(operation);
            }
        }
        if (operationModel == null) return new KeyValuePair<OperationDomainModel, DateTime>(null, now);
        // Else check when to reschedule
        DateTime rescheduleTime = FindRescheduleTime(busySchedule, patientSchedule, duration);
        return new KeyValuePair<OperationDomainModel, DateTime>(operationModel, rescheduleTime);
    }

    public DateTime FindRescheduleTime(List<KeyValuePair<DateTime, DateTime>> busySchedule,
        List<KeyValuePair<DateTime, DateTime>> patientSchedule, decimal duration)
    {
        // Can't do logic, will reschedule after larger schedule[-1]
        var doctor = busySchedule.Last();
        var patient = patientSchedule.Last();
        if (doctor.Value > patient.Value) return doctor.Value;
        return patient.Value;
    }
}