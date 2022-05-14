using System.Linq.Expressions;
using HealthCare.Data.Entities;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using HealthCare.Repositories;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections.Generic;
using System.Linq;
using HealthCare.Domain.DTOs;

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
    private IDoctorRepository _doctorRepository;

    public ExaminationService(IExaminationRepository examinationRepository,
                              IExaminationApprovalRepository examinationApprovalRepository,
                              IOperationRepository operationRepository,
                              IRoomRepository roomRepository,
                              IAntiTrollRepository antiTrollRepository,
                              IAnamnesisRepository anamnesisRepository,
                              IPatientRepository patientRepository,
                              IDoctorRepository doctorRepository)
    {
        _examinationRepository = examinationRepository;
        _examinationApprovalRepository = examinationApprovalRepository;
        _operationRepository = operationRepository;
        _roomRepository = roomRepository;
        _antiTrollRepository = antiTrollRepository;
        _anamnesisRepository = anamnesisRepository;
        _patientRepository = patientRepository;
        _doctorRepository = doctorRepository;
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

    public static ExaminationDomainModel ParseToModel(Examination examination)
    {
        ExaminationDomainModel examinationModel = new ExaminationDomainModel
        {
            Id = examination.Id,
            StartTime = examination.StartTime,
            DoctorId = examination.DoctorId,
            IsDeleted = examination.IsDeleted,
            PatientId = examination.PatientId,
            RoomId = examination.RoomId,
            IsEmergency = examination.IsEmergency
        };
        
        if (examination.Anamnesis != null)
            examinationModel.Anamnesis = AnamnesisService.ParseToModel(examination.Anamnesis);
        
        return examinationModel;
    }

    public static Examination ParseFromModel(ExaminationDomainModel examinationModel)
    {
        Examination examination = new Examination
        {
            Id = examinationModel.Id,
            StartTime = examinationModel.StartTime,
            DoctorId = examinationModel.DoctorId,
            IsDeleted = examinationModel.IsDeleted,
            PatientId = examinationModel.PatientId,
            RoomId = examinationModel.RoomId,
            IsEmergency = examinationModel.IsEmergency
        };
        
        if (examinationModel.Anamnesis != null)
            examination.Anamnesis = AnamnesisService.ParseFromModel(examinationModel.Anamnesis);
        
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
            results.Add(ParseToModel(item));
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
            results.Add(ParseToModel(item));
        }

        return results;
    }

    public async Task<IEnumerable<ExaminationDomainModel>> GetAllForPatientSorted(SortExaminationDTO dto, IDoctorService doctorService)
    {
        List<ExaminationDomainModel> examinations;
        try
        {
            examinations = (List<ExaminationDomainModel>) await GetAllForPatient(dto.PatientId);
        }
        catch (Exception)
        {
            throw new DataIsNullException();
        }
        
        if (dto.SortParam.Equals("date"))
            return examinations.OrderBy(x => x.StartTime);
        
        if (dto.SortParam.Equals("doctor"))
            return examinations.OrderBy(x => x.DoctorId);
        
        Dictionary<decimal, decimal> doctorsSpecialisations = await MapSpecializations(examinations, doctorService);
        return examinations.OrderBy(x => doctorsSpecialisations[x.DoctorId]);
    }

    public async Task<Dictionary<decimal, decimal>> MapSpecializations(List<ExaminationDomainModel> examinations, IDoctorService doctorService)
    {
        Dictionary<decimal, decimal> result = new Dictionary<decimal, decimal>();
        foreach (var examination in examinations)
        {
            if (result.ContainsKey(examination.DoctorId)) continue;
            DoctorDomainModel doctor = await doctorService.GetById(examination.DoctorId);
            result.Add(examination.DoctorId, doctor.SpecializationId);
        }

        return result;
    }

    public async Task<IEnumerable<ExaminationDomainModel>> GetAllForDoctor(decimal id)
    {
        IEnumerable<Examination> data = await _examinationRepository.GetAllByDoctorId(id);
        if (data == null)
            throw new DataIsNullException();

        List<ExaminationDomainModel> results = new List<ExaminationDomainModel>();
        foreach (Examination item in data)
        {
            results.Add(ParseToModel(item));
        }

        return results;
    }

    public async Task<ExaminationDomainModel> Delete(DeleteExaminationDTO dto)
    {
        if (dto.IsPatient && await AntiTrollCheck(dto.PatientId, false))
            throw new DataIsNullException();

        Examination examination = await _examinationRepository.GetExamination(dto.ExaminationId);
        double daysUntilExamination = (examination.StartTime - DateTime.Now).TotalDays;

        if (daysUntilExamination > 1 || !dto.IsPatient)
            DeleteExamination(examination);
        else
            CreateExaminationApproval(examination.Id, examination.Id);

        if (dto.IsPatient)
            WriteToAntiTroll(examination.PatientId, "deleted");
        
        return ParseToModel(examination);
    }

    public void WriteToAntiTroll(decimal patientId, string state)
    {
        AntiTroll antiTrollItem = new AntiTroll
        {
            PatientId = patientId,
            State = state,
            DateCreated = removeSeconds(DateTime.Now)
        };

        _ = _antiTrollRepository.Post(antiTrollItem);
        _antiTrollRepository.Save();
    }

    public void CreateExaminationApproval(decimal oldId, decimal newId)
    {
        ExaminationApproval examinationApproval = new ExaminationApproval
        {
            State = "created",
            IsDeleted = false,
            NewExaminationId = newId,
            OldExaminationId = oldId 
        };
        _ = _examinationApprovalRepository.Post(examinationApproval);
        _examinationApprovalRepository.Save();
    }

    public void DeleteExamination(Examination examination)
    {
        examination.IsDeleted = true;
        _ = _examinationRepository.Update(examination);
        _examinationRepository.Save();

        // anamnesis can't exist without its examination
        // check if anamnesis exists
        if (examination.Anamnesis == null) return;
        
        examination.Anamnesis.IsDeleted = true;
        _ = _anamnesisRepository.Update(examination.Anamnesis);
        _anamnesisRepository.Save();
    }

    private async Task<bool> isPatientOnExamination(CUExaminationDTO dto)
    {
        IEnumerable<Examination> patientsExaminations = await _examinationRepository.GetAllByPatientId(dto.PatientId);
        foreach (Examination examination in patientsExaminations)
        {
            if (examination.Id != dto.ExaminationId)
            {
                double difference = (dto.StartTime - examination.StartTime).TotalMinutes;
                if (difference <= 15 && difference >= -15)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private async Task<bool> isPatientOnOperation(CUExaminationDTO dto)
    {
        IEnumerable<Operation> patientsOperations = await _operationRepository.GetAllByPatientId(dto.PatientId);
        foreach (Operation operation in patientsOperations)
        {
            double difference = (dto.StartTime - operation.StartTime).TotalMinutes;
            if (difference <= (double)operation.Duration && difference >= -15)
            {
                return true;
            }
        }
        return false;
    }

    private async Task<bool> isDoctorOnExamination(CUExaminationDTO dto)
    {
        IEnumerable<Examination> doctorsExaminations = await _examinationRepository.GetAllByDoctorId(dto.DoctorId);
        if (doctorsExaminations == null)
        {
            return false;
        }
        foreach (Examination examination in doctorsExaminations) {
            if (examination.Id != dto.ExaminationId)
            {
                double difference = (dto.StartTime - examination.StartTime).TotalMinutes;
                if (difference <= 15 && difference >= -15)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private async Task<bool> isDoctorOnOperation(CUExaminationDTO dto)
    {
        IEnumerable<Operation> doctorsOperations = await _operationRepository.GetAllByDoctorId(dto.DoctorId);
        foreach (Operation operation in doctorsOperations)
        {
            double difference = (dto.StartTime - operation.StartTime).TotalMinutes;
            if (difference <= (double)operation.Duration && difference >= -15)
            {
                return true;
            }
        }
        return false;
    }

    private async Task<bool> isRoomAvailable(decimal id, DateTime startTime)
    {
        bool isRoomAvailable = true;
        IEnumerable<Examination> examinations = await _examinationRepository.GetAllByRoomId(id);
        foreach (Examination examination in examinations)
        {
            double difference = (startTime - examination.StartTime).TotalMinutes;
            if (difference <= 15 && difference >= -15)
            {
                isRoomAvailable = false;
                break;
            }
        }

        return isRoomAvailable;
    }

    private async Task<decimal> getAvailableRoomId(DateTime startTime)
    {
        IEnumerable<Room> rooms = await _roomRepository.GetAllAppointmentRooms("examination");
        foreach (Room room in rooms)
        {
            bool roomAvailable = await isRoomAvailable(room.Id, startTime);
            if (roomAvailable)
            {
                return room.Id;
            }
        }
        return -1;
    }


    private async Task<bool> isDoctorAvailable(CUExaminationDTO dto)
    {
        return !(await isDoctorOnExamination(dto) ||
                 await isDoctorOnOperation(dto));
    }

    private async Task<bool> isPatientAvailable(CUExaminationDTO dto)
    {
        return !(await isPatientOnExamination(dto) ||
                 await isPatientOnOperation(dto));
    }

    public async Task<ExaminationDomainModel> Create(CUExaminationDTO dto)
    {
        await validateUserInput(dto);

        if (dto.IsPatient && await AntiTrollCheck(dto.PatientId, true))
            throw new AntiTrollException();

        decimal roomId = await getAvailableRoomId(dto.StartTime);
        if (roomId == -1)
            throw new NoFreeRoomsException();

        if (dto.IsPatient) 
            WriteToAntiTroll(dto.PatientId, "create");

        Examination newExamination = new Examination 
        {
            PatientId = dto.PatientId,
            RoomId = roomId,
            DoctorId = dto.DoctorId,
            StartTime = removeSeconds(dto.StartTime),
            IsDeleted = false,
            Anamnesis = null
        };
        _ = _examinationRepository.Post(newExamination);
        _examinationRepository.Save();

        return ParseToModel(newExamination);
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

    private async Task validateUserInput(CUExaminationDTO dto)
    {
        if (dto.StartTime <= DateTime.Now)
            throw new DateInPastExeption();
        if (await isPatientBlocked(dto.PatientId))
            throw new PatientIsBlockedException();

        bool doctorAvailable = await isDoctorAvailable(dto);
        if (!doctorAvailable)
            throw new DoctorNotAvailableException();

        bool patientAvailable = await isPatientAvailable(dto);
        if (!patientAvailable)
            throw new PatientNotAvailableException();
    }
    public async Task<ExaminationDomainModel> Update(CUExaminationDTO dto) 
    {
        await validateUserInput(dto);

        // One patient can't change other patient's appointment
        // so the patient will always match examinationModel.PatientId
        if (dto.IsPatient && await AntiTrollCheck(dto.PatientId, false))
            throw new AntiTrollException();
        
        Examination examination = await _examinationRepository.GetExaminationWithoutAnamnesis(dto.ExaminationId);
        double daysUntilExamination = (examination.StartTime - DateTime.Now).TotalDays;

        decimal roomId = await getAvailableRoomId(dto.StartTime);
        if (roomId == -1)
            throw new NoFreeRoomsException();

        if (daysUntilExamination > 1 || !dto.IsPatient)
            UpdateExamination(dto, roomId, examination);
        else 
        {
            Examination newExamination = CreateExamination(dto, roomId);
            Examination createdExamination = await _examinationRepository.GetByParams(newExamination.DoctorId, newExamination.RoomId, newExamination.PatientId, removeSeconds(newExamination.StartTime));
            // Make an approval request
            CreateExaminationApproval(examination.Id, createdExamination.Id);
        }
            

        if (dto.IsPatient) 
            WriteToAntiTroll(dto.PatientId, "update");

        return ParseToModel(examination);
    }

    public void UpdateExamination(CUExaminationDTO dto, decimal roomId, Examination examination)
    {
        examination.RoomId = roomId;
        examination.DoctorId = dto.DoctorId;
        examination.PatientId = dto.PatientId;
        examination.StartTime = removeSeconds(dto.StartTime);
        //update
        _ = _examinationRepository.Update(examination);
        _examinationRepository.Save();
    }

    public Examination CreateExamination(CUExaminationDTO dto, decimal roomId)
    {
        Examination newExamination = new Examination 
        {
            PatientId = dto.PatientId,
            RoomId = roomId,
            DoctorId = dto.DoctorId,
            StartTime = dto.StartTime,
            IsDeleted = true,
            Anamnesis = null
        };

        _ = _examinationRepository.Post(newExamination);
        _examinationRepository.Save();
        return newExamination;
    }

    private async Task<List<KeyValuePair<DateTime, DateTime>>> getScehdule(ParamsForRecommendingFreeExaminationsDTO paramsDTO, IDoctorService doctorService)
    {
        IEnumerable<KeyValuePair<DateTime, DateTime>> freeTimes = await doctorService.GetAvailableSchedule(paramsDTO.DoctorId);
        List<KeyValuePair<DateTime, DateTime>> possibleSlots = new List<KeyValuePair<DateTime, DateTime>>();
        foreach (KeyValuePair<DateTime, DateTime> time in freeTimes)
        {
            if (DateTime.Now < time.Value && time.Key < paramsDTO.LastDate)
            {
                possibleSlots.Add(time);
            }
        }
        return possibleSlots;
    }


    private async Task<CUExaminationDTO> checkAviabilityForExamination(ParamsForRecommendingFreeExaminationsDTO paramsDTO, DateTime startTime)
    {

        CUExaminationDTO dto = new CUExaminationDTO
        {
            DoctorId = paramsDTO.DoctorId,
            PatientId = paramsDTO.PatientId,
            StartTime = startTime
        };

        try
        {
            await validateUserInput(dto);
        }
        catch (Exception ex)
        {
            return null;
        }
        return dto;
    }

    private async Task<List<CUExaminationDTO>> getRecommendedExaminationsForOneDoctor(ParamsForRecommendingFreeExaminationsDTO paramsDTO, IDoctorService doctorService)
    {
        List<CUExaminationDTO> recommendedExaminaions = new List<CUExaminationDTO>();
        List<KeyValuePair<DateTime, DateTime>> possibleSlots = await getScehdule(paramsDTO, doctorService);
        if (possibleSlots.Count == 0) return null;

        int numOfExaminations = 0;
        int possibleSlotIndex = 0;

        DateTime startTime = DateTime.Now;
        //DateTime startTime = paramsDTO.TimeFrom;
        //if (startTime.TimeOfDay < possibleSlots[possibleSlotIndex].Key.TimeOfDay)
        //    startTime = possibleSlots[possibleSlotIndex].Key;
        //else
        //    startTime = DateTime.Now;

        while (numOfExaminations != 3)
        {
            //start time is in available range for doctor and patient
            if (startTime.TimeOfDay < paramsDTO.TimeTo.TimeOfDay && startTime < possibleSlots[possibleSlotIndex].Value)
            {
                CUExaminationDTO recommendedExamination = await checkAviabilityForExamination(paramsDTO, startTime);
                if (recommendedExamination != null)
                {
                    recommendedExaminaions.Add(recommendedExamination);
                    numOfExaminations++;
                    if (numOfExaminations == 3)
                    {
                        break;
                    }
                }
                startTime = startTime.AddMinutes(15);
            }
            else
            {
                //there is another day in doctor free range 
                if (startTime < possibleSlots[possibleSlotIndex].Value)
                {
                    startTime = startTime.AddDays(1);
                    startTime = new DateTime(startTime.Year, startTime.Month, startTime.Day, paramsDTO.TimeFrom.Hour, paramsDTO.TimeFrom.Minute, paramsDTO.TimeFrom.Second);
                    if (startTime > paramsDTO.LastDate)
                    {
                        break;
                    }
                }
                //there is no times in this slot
                else
                {
                    possibleSlotIndex++;
                    if (possibleSlotIndex == possibleSlots.Count)
                    {
                        break;
                    }
                    if (startTime.TimeOfDay < possibleSlots[possibleSlotIndex].Key.TimeOfDay)
                        startTime = possibleSlots[possibleSlotIndex].Key;
                }
            }
        }
        return recommendedExaminaions;
    }

    public async Task<IEnumerable<CUExaminationDTO>> RecommendedByDatePriority(ParamsForRecommendingFreeExaminationsDTO paramsDTO, IDoctorService doctorService, decimal numOfExaminations)
    {
        List<CUExaminationDTO> recommendedExaminations = await getRecommendedExaminationsForOneDoctor(paramsDTO, doctorService);
        DoctorDomainModel doctorModel = await doctorService.GetById(paramsDTO.DoctorId);
        List<DoctorDomainModel> otherDoctors = (List<DoctorDomainModel>)await doctorService.GetAllBySpecialization(doctorModel.SpecializationId);
        int numOfDoctor = 0;
        while (numOfExaminations < 3)
        {
            if (paramsDTO.DoctorId == otherDoctors.ElementAt(numOfDoctor).Id)
            {
                numOfDoctor++;
                continue;
            }
            paramsDTO.DoctorId = otherDoctors.ElementAt(numOfDoctor++).Id;
            List <CUExaminationDTO> newDoctorExaminations = await getRecommendedExaminationsForOneDoctor(paramsDTO, doctorService);
            if (newDoctorExaminations == null)
                continue;
            foreach (var examination in newDoctorExaminations)
            { 
                recommendedExaminations.Add(examination);
                numOfExaminations++;
            }

            if (numOfDoctor > otherDoctors.Count - 1)
                break;
        }
        return recommendedExaminations;
    }

    public async Task<IEnumerable<CUExaminationDTO>> RecommendedByDoctorPriority(ParamsForRecommendingFreeExaminationsDTO paramsDTO, decimal numOfExaminations, DateTime startTime)
    {
        List<CUExaminationDTO> recommendedExaminations = new List<CUExaminationDTO>();
        while (numOfExaminations != 3)
        {
            CUExaminationDTO recommendedExamination = await checkAviabilityForExamination(paramsDTO, startTime);
            if (recommendedExamination != null)
            {
                recommendedExaminations.Add(recommendedExamination);
                numOfExaminations++;
                if (numOfExaminations == 3)
                    break;
            }

            startTime = startTime.AddMinutes(15);
        }

        return recommendedExaminations;
    }
    public async Task<IEnumerable<CUExaminationDTO>> GetRecommendedExaminations(ParamsForRecommendingFreeExaminationsDTO paramsDTO, IDoctorService doctorService)
    {
        List<CUExaminationDTO> recommendedExaminations = await getRecommendedExaminationsForOneDoctor(paramsDTO, doctorService);
        int numOfExaminations = recommendedExaminations.Count;
        if (numOfExaminations != 3)
        {
            if (paramsDTO.IsDoctorPriority)
            {
                if (numOfExaminations == 0)
                    return null;
                // Doctor priority
                DateTime startTime = recommendedExaminations[numOfExaminations - 1].StartTime.AddMinutes(15);
                foreach (var examination in await RecommendedByDoctorPriority(paramsDTO, numOfExaminations, startTime))
                    recommendedExaminations.Add(examination);
            }
            else
            {
                // Date priority
                foreach (var examination in await RecommendedByDatePriority(paramsDTO, doctorService, numOfExaminations))
                    recommendedExaminations.Add(examination);
            }
        }

        return recommendedExaminations;
    }

    public bool IsInAnamnesis(Anamnesis anamnesis, string subString)
    {
        return anamnesis != null && anamnesis.Description.ToLower().Contains(subString);
    }
    public async Task<IEnumerable<ExaminationDomainModel>> SearchByAnamnesis(SearchByNameDTO dto)
    {
        dto.Substring = dto.Substring.ToLower();
        IEnumerable<Examination> examinations = await _examinationRepository.GetByPatientId(dto.PatientId);
        if (examinations == null)
            throw new DataIsNullException();

        List<ExaminationDomainModel> results = new List<ExaminationDomainModel>();

        foreach (Examination item in examinations)
            if(IsInAnamnesis(item.Anamnesis, dto.Substring))
                results.Add(ParseToModel(item));
        
        return results;
       
    }

    public async Task<DateTime?> FirstStartTime(List<KeyValuePair<DateTime, DateTime>> schedule, decimal duration)
    {
        DateTime now = DateTime.Now;
        DateTime limit = removeSeconds(now.AddHours(2));
        foreach (KeyValuePair<DateTime, DateTime> pair in schedule)
        {
            // Now: 20:00, Limit: 22:00, Schedule: 14:00 - 16:00 -> continue
            if (now > pair.Value) continue;
            // Now: 20:00, Limit: 22:00, Schedule: 15:00 - 21:00 -> 20:00
            if (now >= pair.Key && now <= pair.Value && (pair.Value - now).Minutes >= duration) return now;
            // Now: 20:00, Limit: 22:00, Schedule: 21:00 - 23:00 -> 21:00
            if (limit >= pair.Key && pair.Key > now && (pair.Value - pair.Key).Minutes >= duration) return pair.Key;
            // Now: 20:00, Limit: 22:00, Schedule: 23:00 - 23:30 -> break completely (every other
            // pair will be greater than this one, so return null)
            return null;
        }
        return null;
    }

    // DoctorService is needed for doctor's schedule
    public async Task<ExaminationDomainModel> CreateUrgent(CreateUrgentExaminationDTO dto, IDoctorService doctorService,
        INotificationService notificationService)
    {
        ExaminationDomainModel examinationModel = new ExaminationDomainModel
        {
            IsDeleted = false,
            IsEmergency = true,
            PatientId = dto.PatientId
        };
        // Find examination in the first 2 hours for any doctor that matches the specialization criteria
        List<Doctor> doctors = (List<Doctor>) await _doctorRepository.GetBySpecialization(dto.SpecializationId);
        if (doctors == null || doctors.Count == 0) throw new NoAvailableSpecialistsException();
        // Find start times (to sort by earliest) 
        List<KeyValuePair<DateTime, decimal>> urgentStartTimes = await GetUrgentStartTimes(doctors, doctorService);
        
        urgentStartTimes.Sort((x, y) => x.Key.CompareTo(y.Key));
        // Try to create examination
        ExaminationDomainModel? createdModel = await ParsePairs(examinationModel, urgentStartTimes);
        _ = await SendNotifications(notificationService, examinationModel.DoctorId, examinationModel.PatientId);
        return createdModel;
    }

    public async Task<Boolean> TryCreateExamination(ExaminationDomainModel examinationModel)
    {
        decimal roomId = await getAvailableRoomId(examinationModel.StartTime);
        if (roomId == -1) return false;
        examinationModel.RoomId = roomId;
        Examination examination = ParseFromModel(examinationModel);
        _ = _examinationRepository.Post(examination);
        _examinationRepository.Save();
        return true;
    }

    public async Task<ExaminationDomainModel?> ParsePairs(ExaminationDomainModel examinationModel, List<KeyValuePair<DateTime, decimal>> urgentStartTimes)
    {
        Boolean flag = false;
        foreach (KeyValuePair<DateTime, decimal> pair in urgentStartTimes)
        {
            examinationModel.StartTime = removeSeconds(pair.Key);
            examinationModel.DoctorId = pair.Value;
            flag  = await TryCreateExamination(examinationModel);
            if (flag) return examinationModel;
        }
        return null;
    }

    public async Task<List<KeyValuePair<DateTime, decimal>>> GetUrgentStartTimes(List<Doctor> doctors, IDoctorService doctorService)
    {
        List<KeyValuePair<DateTime, decimal>> result = new List<KeyValuePair<DateTime, decimal>>();
        foreach (Doctor doctor in doctors)
        {
            var schedule = (List<KeyValuePair<DateTime, DateTime>>)await doctorService.GetAvailableSchedule(doctor.Id);
            DateTime? startTime = await FirstStartTime(schedule, 15);
            if (startTime.HasValue)
                result.Add(new KeyValuePair<DateTime, decimal>(startTime.GetValueOrDefault(), doctor.Id));
        }
        return result;
    }

    public async Task<IEnumerable<IEnumerable<RescheduleDTO>>> FindFiveAppointments(CreateUrgentExaminationDTO dto, 
        IDoctorService doctorService, IPatientService patientService)
    {
        // For every doctor try to find a single reschedule 
        List<Doctor> doctors = (List<Doctor>) await _doctorRepository.GetAll();
        List<List<List<RescheduleDTO>>> reschedule = new List<List<List<RescheduleDTO>>>();
        foreach (Doctor doctor in doctors)
            reschedule.Add(await GetRescheduleForDoctor(dto, doctor.Id, doctorService, patientService));
        List<KeyValuePair<DateTime, List<RescheduleDTO>>> rescheduleSorted = new List<KeyValuePair<DateTime, List<RescheduleDTO>>>();
        foreach (List<List<RescheduleDTO>> item in reschedule)
            rescheduleSorted.AddRange(await FindRescheduleTime(item, doctorService, patientService, dto.PatientId));
        rescheduleSorted.Sort((x, y) => x.Key.CompareTo(y.Key));
        List<List<RescheduleDTO>> result = new List<List<RescheduleDTO>>();
        foreach (var item in rescheduleSorted)
        {
            result.Add(item.Value);
            if (result.Count > 5) break;
        }
        return result;
    }

    public async Task<List<KeyValuePair<DateTime, List<RescheduleDTO>>>> FindRescheduleTime(List<List<RescheduleDTO>> schedule, 
        IDoctorService doctorService, IPatientService patientService, decimal patientId)
    {
        List<KeyValuePair<DateTime, DateTime>> freePatientSchedule = 
            (List<KeyValuePair<DateTime, DateTime>>) await patientService.GetSchedule(patientId);
        decimal doctorId = schedule[0][0].DoctorId;
        List<KeyValuePair<DateTime, DateTime>> freeDoctorSchedule =
            (List<KeyValuePair<DateTime, DateTime>>) await doctorService.GetAvailableSchedule(doctorId);
        List<KeyValuePair<DateTime, List<RescheduleDTO>>> result = new List<KeyValuePair<DateTime, List<RescheduleDTO>>>();
        foreach (List<RescheduleDTO> sequence in schedule)
        {
            _ = await SetRescheduleForSequence(sequence, freePatientSchedule, freeDoctorSchedule);
            DateTime max = await FindMaxDateInSequence(sequence);
            result.Add(new KeyValuePair<DateTime, List<RescheduleDTO>>(max, sequence));
        }
        return result;
    }

    public async Task<DateTime> FindMaxDateInSequence(List<RescheduleDTO> sequence)
    {
        DateTime max = DateTime.Now;
        foreach (RescheduleDTO item in sequence)
            if (item.RescheduleTime > max) max = item.RescheduleTime;
        return max;
    }

    public async Task<Boolean> SetRescheduleForSequence(List<RescheduleDTO> sequence, 
        List<KeyValuePair<DateTime, DateTime>> patientSchedule,
        List<KeyValuePair<DateTime, DateTime>> doctorSchedule)
    {
        foreach (RescheduleDTO item in sequence)
            _ = await SetRescheduleForDTO(item, patientSchedule, doctorSchedule);
        return true;
    }

    public int GetIndex(List<KeyValuePair<DateTime, DateTime>> schedule, DateTime reference)
    {
        for (int i = 0; i < schedule.Count; i++)
            if (schedule[i].Key > reference)
                return i;
        return 0;
    }

    public async Task<Boolean> SetRescheduleForDTO(RescheduleDTO dto,
        List<KeyValuePair<DateTime, DateTime>> patientSchedule,
        List<KeyValuePair<DateTime, DateTime>> doctorSchedule)
    {
        int patientIndex = GetIndex(patientSchedule, dto.StartTime);
        int doctorIndex = GetIndex(patientSchedule, dto.StartTime);

        Boolean found = false;
        while (!found)
        {
            KeyValuePair<DateTime, DateTime> doctorPair = doctorSchedule[doctorIndex];
            KeyValuePair<DateTime, DateTime> patientPair = patientSchedule[patientIndex];
            if (!IsDateTimeOverlap(doctorPair, patientPair))
            {
                // Update smaller
                if (doctorPair.Key < patientPair.Key && doctorPair.Value < patientPair.Value)
                    doctorIndex++;
                else
                    patientIndex++;
                continue;
            }

            DateTime rescheduleTime = CalculateRescheduleTime(doctorPair, patientPair, dto.Duration);
            if (rescheduleTime == DateTime.MaxValue) continue;
            dto.RescheduleTime = rescheduleTime;
            found = true;
        }
        return true;
    }

    public DateTime CalculateRescheduleTime(KeyValuePair<DateTime, DateTime> first, KeyValuePair<DateTime, DateTime> second, decimal duration)
    {
        decimal window = 0;
        if (first.Key < second.Key)
        {
            if (first.Value < second.Value)
                window = (first.Value - second.Key).Minutes;
            else
                window = (second.Value - second.Key).Minutes;
            
            if (window >= duration)
                return second.Key;
        }
        if (first.Value > second.Value)
            window = (second.Value - first.Key).Minutes;
        else
            window = (first.Value - first.Key).Minutes;
        
        if (window >= duration)
            return first.Key;
        
        return DateTime.MaxValue;
    }
    
    public async Task<List<List<RescheduleDTO>>> GetRescheduleForDoctor(CreateUrgentExaminationDTO dto, decimal doctorId,
        IDoctorService doctorService, IPatientService patientService, decimal duration=15)
    {
        List<KeyValuePair<DateTime, DateTime>> freeSchedule = 
            (List<KeyValuePair<DateTime, DateTime>>) await doctorService.GetAvailableSchedule(doctorId);
        List<KeyValuePair<DateTime, DateTime>> busySchedule = 
            (List<KeyValuePair<DateTime, DateTime>>) await doctorService.GetBusySchedule(doctorId);
        // Loop variables
        DateTime now = removeSeconds(DateTime.Now);
        DateTime new_now = now;
        DateTime limit = removeSeconds(DateTime.Now.AddHours(2));
        DateTime first, second;
        int index = GetFirstIndex(freeSchedule, false);
        int size = 0;

        List<List<RescheduleDTO>> result = new List<List<RescheduleDTO>>();
        List<RescheduleDTO> tempList = new List<RescheduleDTO>();
        if (index == -1)
            // If doctor has no free room in his schedule
            return CalculateWithNoFreeTime(busySchedule, dto.PatientId, doctorId, duration);
        
        // If doctor has free time in his schedule
        int busyIndex = GetFirstIndex(busySchedule, true);
        while (index != -1 && busyIndex != -1)
        {
            bool flagFree = false; 
            KeyValuePair<DateTime, DateTime> freePair = freeSchedule[index];
            KeyValuePair<DateTime, DateTime> busyPair = busySchedule[busyIndex];
            if (freePair.Value == busyPair.Key)
            {
                flagFree = true;
                new_now = busyPair.Value;
            }
            else if (freePair.Key == busyPair.Value)
            {
                flagFree = true;
                new_now = freePair.Value;
            }
            else
                new_now = busyPair.Key;
            
            int old_free = index;
            int old_busy = busyIndex;
            result.Add(FindSequence(freeSchedule, busySchedule, index, busyIndex, duration, now, dto.PatientId, doctorId));
            
            if (flagFree)
                if(UpdateIndex(freeSchedule, old_free) != -1) index = UpdateIndex(freeSchedule, old_free);
            if(UpdateIndex(busySchedule, old_busy) != -1) busyIndex = UpdateIndex(busySchedule, old_busy);
            if (new_now > limit) break;
            now = new_now;
        }
        return result;
    }

    public List<RescheduleDTO> FindSequence(List<KeyValuePair<DateTime, DateTime>> freeSchedule, List<KeyValuePair<DateTime, DateTime>> busySchedule,
        int index, int busyIndex, decimal duration, DateTime now, decimal patientId, decimal doctorId)
    {
        int size = 0;
        DateTime rescheduleTime = now;
        List<RescheduleDTO> sequence = new List<RescheduleDTO>();
        while (size < duration || (index == -1 && busyIndex == -1))
        {
            Boolean flagFree = false;
            KeyValuePair<DateTime, DateTime> freePair = freeSchedule[index];
            KeyValuePair<DateTime, DateTime> busyPair = busySchedule[busyIndex];
            DateTime first, second;
            // Max possible range (if rescheduled)
            if (freePair.Value == busyPair.Key)
            {
                first = freePair.Key;
                second = busyPair.Value;
                flagFree = true;
            }
            else if (freePair.Key == busyPair.Value)
            {
                first = busyPair.Key;
                second = freePair.Value;
                flagFree = true;
            }
            else
            {
                first = busyPair.Key;
                second = busyPair.Value;
            }
            size += (second - now).Minutes;
            sequence.Add(new RescheduleDTO{PatientId = patientId, DoctorId = doctorId, StartTime = second, EndTime = first, UrgentStartTime = rescheduleTime});
            now = first;
            // Update
            if (flagFree)
                if(UpdateIndex(freeSchedule, index) != -1) index = UpdateIndex(freeSchedule, index);
            if(UpdateIndex(busySchedule, busyIndex) != -1) busyIndex = UpdateIndex(busySchedule, busyIndex);
        }

        return sequence;
    }

    public List<List<RescheduleDTO>> CalculateWithNoFreeTime(List<KeyValuePair<DateTime, DateTime>> busySchedule, 
        decimal patientId, decimal doctorId, decimal duration)
    {
        List<RescheduleDTO> tempList = new List<RescheduleDTO>();
        List<List<RescheduleDTO>> result = new List<List<RescheduleDTO>>();
        DateTime first, second, now = DateTime.Now;
        decimal size = 0;
        for (var i = GetFirstIndex(busySchedule, true); i < busySchedule.Count-1; i++)
        {
            first = busySchedule[i].Value;
            second = busySchedule[i+1].Key;
            size = 0;
            DateTime rescheduleTime = now;
            while (size < duration)
            {
                size += (second - now).Minutes;
                tempList.Add(new RescheduleDTO{PatientId = patientId, DoctorId = doctorId, StartTime = second, EndTime = first, UrgentStartTime = rescheduleTime});
            }
            now = first;
            result.Add(tempList);
        }

        return result;
    }

    public int UpdateIndex(List<KeyValuePair<DateTime, DateTime>> schedule, int lastIndex)
    {
        if (lastIndex + 1 == schedule.Count) return -1;
        KeyValuePair<DateTime, DateTime> pair = schedule[lastIndex + 1];
        if (pair.Key > removeSeconds(DateTime.Now).AddHours(2)) return -1;
        return lastIndex + 1;
    }
    
    public int GetFirstIndex(List<KeyValuePair<DateTime, DateTime>> schedule, bool isBusy)
    {
        DateTime now = removeSeconds(DateTime.Now);
        DateTime limit = now.AddHours(2);
        for (var i = 0; i < schedule.Count; i++)
        {
            KeyValuePair<DateTime, DateTime> pair = schedule[i];
            if (pair.Key > limit) break;
            if (isBusy && pair.Value > now) return i;
            if (!isBusy && pair.Key >= now) return i;
        }
        
        return -1;
    }
    private bool IsDateTimeOverlap(KeyValuePair<DateTime, DateTime> first, KeyValuePair<DateTime, DateTime> second)
    {
        return MaxDate(first.Key, second.Key) < MinDate(first.Value, second.Value);

    }

    private DateTime MaxDate(DateTime time1, DateTime time2)
    {
        return (time1 > time2 ? time1 : time2);
    }

    private DateTime MinDate(DateTime time1, DateTime time2)
    {
        return (time1 < time2 ? time1 : time2);
    }

    public async Task<ExaminationDomainModel> AppointUrgent(List<RescheduleDTO> dto, INotificationService notificationService)
    {
        foreach (RescheduleDTO item in dto)
            _ = await RescheduleOne(item, notificationService);
        // Any dto will do
        return await MakeUrgent(dto[0]);
    }

    public async Task<ExaminationDomainModel> RescheduleOne(RescheduleDTO dto, INotificationService notificationService)
    {
        Examination examination = await _examinationRepository.GetByParams(dto.DoctorId, dto.PatientId, dto.StartTime);
        examination.StartTime = dto.RescheduleTime;
        _ = _examinationRepository.Update(examination);
        _examinationRepository.Save();
        _ = await SendNotifications(notificationService, dto.DoctorId, dto.PatientId);
        return ParseToModel(examination);
    }

    public async Task<Boolean> SendNotifications(INotificationService notificationService, decimal doctorId=0, decimal patientId=0)
    {
        KeyValuePair<string, string> content = new KeyValuePair<string, string>("Rescheduling",
            "Your appointment has been rescheduled. Please check your schedule");
       if (doctorId != 0) 
           _ = await notificationService.Send(new SendNotificationDTO{IsPatient = false, Content = content, PersonId = doctorId});
       if (patientId != 0) 
           _ = await notificationService.Send(new SendNotificationDTO{IsPatient = true, Content = content, PersonId = patientId});
       return true;
    }

    public async Task<ExaminationDomainModel> MakeUrgent(RescheduleDTO dto)
    {
        ExaminationDomainModel examinationModel = new ExaminationDomainModel
        {
            DoctorId = dto.DoctorId,
            IsDeleted = false,
            IsEmergency = true,
            StartTime = dto.UrgentStartTime,
            PatientId = dto.PatientId,
            RoomId = await getAvailableRoomId(dto.UrgentStartTime)
        };
        _ = _examinationRepository.Post(ParseFromModel(examinationModel));
        _examinationRepository.Save();
        return examinationModel;
    }
}
