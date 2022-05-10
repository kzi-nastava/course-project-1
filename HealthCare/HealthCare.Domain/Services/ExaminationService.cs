using HealthCare.Data.Entities;
using HealthCare.Domain.DataTransferObjects;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using HealthCare.Repositories;
using System.Collections.Generic;
using System.Linq;

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
            RoomId = examination.RoomId,
            IsEmergency = examination.IsEmergency
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
            RoomId = examinationModel.RoomId,
            IsEmergency = examinationModel.IsEmergency
        };
        if (examinationModel.Anamnesis != null)
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

    public async Task<IEnumerable<ExaminationDomainModel>> GetAllForPatientSorted(decimal id, string sortParam, IDoctorService doctorService)
    {
        IEnumerable<ExaminationDomainModel> examinations = await GetAllForPatient(id);
        IEnumerable<ExaminationDomainModel> sortedExaminations = null;
        if (sortParam.Equals("date"))
            sortedExaminations = examinations.OrderBy(x => x.StartTime);
        else if (sortParam.Equals("doctor"))
            sortedExaminations = examinations.OrderBy(x => x.DoctorId);
        else
        {
            Dictionary<decimal, decimal> doctorsSpecialisations = new Dictionary<decimal, decimal>();
            foreach (var examination in examinations)
            {
                DoctorDomainModel doctor = await doctorService.GetById(examination.DoctorId);
                try
                { 
                   doctorsSpecialisations.Add(examination.DoctorId, doctor.SpecializationId);
                }
                catch (Exception ex)
                {
                    continue;
                }
            }
            sortedExaminations = examinations.OrderBy(x => doctorsSpecialisations[x.DoctorId]);
        }

        return sortedExaminations;
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

    private async Task<bool> isPatientOnExamination(ExaminationDomainModel examinationModel)
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

    private async Task<bool> isPatientOnOperation(ExaminationDomainModel examinationModel)
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

    private async Task<bool> isDoctorOnExamination(ExaminationDomainModel examinationModel)
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

    private async Task<bool> isDoctorOnOperation(ExaminationDomainModel examinationModel)
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

    private async Task<decimal> getAvailableRoomId(ExaminationDomainModel examinationModel)
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


    private async Task<bool> isDoctorAvailable(ExaminationDomainModel examinationModel)
    {
        return !(await isDoctorOnExamination(examinationModel) ||
                 await isDoctorOnOperation(examinationModel));
    }

    private async Task<bool> isPatientAvailable(ExaminationDomainModel examinationModel)
    {
        return !(await isPatientOnExamination(examinationModel) ||
                 await isPatientOnOperation(examinationModel));
    }

    public async Task<ExaminationDomainModel> Create(ExaminationDomainModel examinationModel, bool isPatient)
    {
        await validateUserInput(examinationModel);

        if (isPatient && await AntiTrollCheck(examinationModel.PatientId, true))
            throw new AntiTrollException();

        decimal roomId = await getAvailableRoomId(examinationModel);
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

        return parseToModel(newExamination);
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

    private async Task validateUserInput(ExaminationDomainModel examinationModel)
    {
        if (examinationModel.StartTime <= DateTime.Now)
            throw new DateInPastExeption();
        if (await isPatientBlocked(examinationModel.PatientId))
            throw new PatientIsBlockedException();
        bool doctorAvailable = await isDoctorAvailable(examinationModel);
        bool patientAvailable = await isPatientAvailable(examinationModel);
        if (!doctorAvailable)
            throw new DoctorNotAvailableException();
        if (!patientAvailable)
            throw new PatientNotAvailableException();
    }
    public async Task<ExaminationDomainModel> Update(ExaminationDomainModel examinationModel, bool isPatient) 
    {
        await validateUserInput(examinationModel);

        // One patient can't change other patient's appointment
        // so the patient will always match examinationModel.PatientId
        if (isPatient && await AntiTrollCheck(examinationModel.PatientId, false))
            throw new AntiTrollException();
        Examination examination = await _examinationRepository.GetExaminationWithoutAnamnesis(examinationModel.Id);
        double daysUntilExamination = (examination.StartTime - DateTime.Now).TotalDays;

        decimal roomId = await getAvailableRoomId(examinationModel);
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
                IsEmergency = examinationModel.IsEmergency

            };

            _ = _examinationRepository.Post(newExamination);
            _examinationRepository.Save();

            Examination createdExamination = await _examinationRepository.GetByParams(newExamination.DoctorId, newExamination.RoomId, newExamination.PatientId, removeSeconds(newExamination.StartTime));

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

        return parseToModel(examination);
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


    private async Task<ExaminationDomainModel> checkAviabilityForExamination(ParamsForRecommendingFreeExaminationsDTO paramsDTO, DateTime startTime)
    {
        ExaminationDomainModel recommendedExamination = new ExaminationDomainModel
        {
            DoctorId = paramsDTO.DoctorId,
            PatientId = paramsDTO.PatientId,
            StartTime = startTime
        };
        try
        {
            await validateUserInput(recommendedExamination);
        }
        catch (Exception ex)
        {
            return null;
        }
        return recommendedExamination;
    }

    private async Task<List<ExaminationDomainModel>> getRecommendedExaminationsForOneDoctor(ParamsForRecommendingFreeExaminationsDTO paramsDTO, IDoctorService doctorService)
    {
        List<ExaminationDomainModel> recommendedExaminaions = new List<ExaminationDomainModel>();
        List<KeyValuePair<DateTime, DateTime>> possibleSlots = await getScehdule(paramsDTO, doctorService);
        if (possibleSlots.Count == 0) return null;

        int numOfExaminations = 0;
        int possibleSlotIndex = 0;

        DateTime startTime = paramsDTO.TimeFrom;
        if (startTime.TimeOfDay < possibleSlots[possibleSlotIndex].Key.TimeOfDay)
            startTime = possibleSlots[possibleSlotIndex].Key;
        else
            startTime = DateTime.Now;
        paramsDTO.TimeTo.AddMinutes(-15);

        while (numOfExaminations != 3)
        {
            //start time is in available range for doctor and patient
            if (startTime.TimeOfDay < paramsDTO.TimeTo.TimeOfDay && startTime < possibleSlots[possibleSlotIndex].Value)
            {
                ExaminationDomainModel recommendedExamination = await checkAviabilityForExamination(paramsDTO, startTime);
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
    

    public async Task<IEnumerable<ExaminationDomainModel>> GetRecommendedExaminations(ParamsForRecommendingFreeExaminationsDTO paramsDTO, IDoctorService doctorService)
    {
        List<ExaminationDomainModel> recommendedExaminaions = await getRecommendedExaminationsForOneDoctor(paramsDTO, doctorService);
        int numOfExaminations = recommendedExaminaions.Count;
        if (numOfExaminations != 3)
        {
            if (paramsDTO.IsDoctorPriority)
            {
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
                    else
                        paramsDTO.DoctorId = otherDoctors.ElementAt(numOfDoctor++).Id;
                    List <ExaminationDomainModel> newDoctorExaminations = await getRecommendedExaminationsForOneDoctor(paramsDTO, doctorService);
                    if (newDoctorExaminations == null)
                        continue;
                    foreach (var examination in newDoctorExaminations)
                    { 
                        recommendedExaminaions.Add(examination);
                        numOfExaminations++;
                    }
                    if(numOfDoctor > otherDoctors.Count - 1)
                    {
                        return recommendedExaminaions;
                    }
                }

            }
            else
            {
                DateTime startTime = recommendedExaminaions[numOfExaminations - 1].StartTime.AddMinutes(15);

                while (numOfExaminations != 3)
                {
                    ExaminationDomainModel recommendedExamination = await checkAviabilityForExamination(paramsDTO, startTime);
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
            }

        }

        return recommendedExaminaions;
    }
    public async Task<IEnumerable<ExaminationDomainModel>> SearchByAnamnesis(decimal id, string substring)
    {
        substring = substring.ToLower();
        IEnumerable<Examination> examinations = await _examinationRepository.GetByPatientId(id);
        if (examinations == null)
            throw new DataIsNullException();

        List<ExaminationDomainModel> results = new List<ExaminationDomainModel>();

        foreach (Examination item in examinations)
        {
            if(item.Anamnesis != null && item.Anamnesis.Description.ToLower().Contains(substring))
                results.Add(parseToModel(item));
        }
        return results;
       
    }
}
