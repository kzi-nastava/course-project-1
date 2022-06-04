using HealthCare.Data.Entities;
using HealthCare.Domain.DTOs;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using HealthCare.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCare.Domain.Services
{
    public class UrgentExaminationService : IUrgentExaminationService
    {

        private IExaminationRepository _examinationRepository;
        private IPatientRepository _patientRepository;
        private IDoctorRepository _doctorRepository;

        public UrgentExaminationService(IExaminationRepository examinationRepository,
                                  IPatientRepository patientRepository,
                                  IDoctorRepository doctorRepository)
        {
            _examinationRepository = examinationRepository;
            _patientRepository = patientRepository;
            _doctorRepository = doctorRepository;
        }

        public async Task<DateTime?> FirstStartTime(List<KeyValuePair<DateTime, DateTime>> schedule, decimal duration)
        {
            DateTime now = DateTime.Now;
            DateTime limit = UtilityService.RemoveSeconds(now.AddHours(2));
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
            INotificationService notificationService, IRoomService roomService)
        {
            ExaminationDomainModel examinationModel = new ExaminationDomainModel
            {
                IsDeleted = false,
                IsEmergency = true,
                PatientId = dto.PatientId
            };
            // Find examination in the first 2 hours for any doctor that matches the specialization criteria
            List<Doctor> doctors = (List<Doctor>)await _doctorRepository.GetBySpecialization(dto.SpecializationId);
            if (doctors == null || doctors.Count == 0) throw new NoAvailableSpecialistsException();
            // Find start times (to sort by earliest) 
            List<KeyValuePair<DateTime, decimal>> urgentStartTimes = await GetUrgentStartTimes(doctors, doctorService);

            urgentStartTimes.Sort((x, y) => x.Key.CompareTo(y.Key));
            // Try to create examination
            ExaminationDomainModel? createdModel = await ParsePairs(examinationModel, urgentStartTimes, roomService);
            _ = await SendNotifications(notificationService, examinationModel.DoctorId, examinationModel.PatientId);
            return createdModel;
        }

        public async Task<Boolean> TryCreateExamination(ExaminationDomainModel examinationModel, IRoomService roomService)
        {
            decimal roomId = await roomService.GetAvailableRoomId(examinationModel.StartTime, "examination");
            if (roomId == -1) return false;
            examinationModel.RoomId = roomId;
            Examination examination = ExaminationService.ParseFromModel(examinationModel);
            _ = _examinationRepository.Post(examination);
            _examinationRepository.Save();
            return true;
        }

        public async Task<ExaminationDomainModel?> ParsePairs(ExaminationDomainModel examinationModel, List<KeyValuePair<DateTime, decimal>> urgentStartTimes, IRoomService roomService)
        {
            Boolean flag = false;
            foreach (KeyValuePair<DateTime, decimal> pair in urgentStartTimes)
            {
                examinationModel.StartTime = UtilityService.RemoveSeconds(pair.Key);
                examinationModel.DoctorId = pair.Value;
                flag = await TryCreateExamination(examinationModel, roomService);
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
            List<Doctor> doctors = (List<Doctor>)await _doctorRepository.GetAll();
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
                (List<KeyValuePair<DateTime, DateTime>>)await patientService.GetSchedule(patientId);
            decimal doctorId = schedule[0][0].DoctorId;
            List<KeyValuePair<DateTime, DateTime>> freeDoctorSchedule =
                (List<KeyValuePair<DateTime, DateTime>>)await doctorService.GetAvailableSchedule(doctorId);
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
            IDoctorService doctorService, IPatientService patientService, decimal duration = 15)
        {
            List<KeyValuePair<DateTime, DateTime>> freeSchedule =
                (List<KeyValuePair<DateTime, DateTime>>)await doctorService.GetAvailableSchedule(doctorId);
            List<KeyValuePair<DateTime, DateTime>> busySchedule =
                (List<KeyValuePair<DateTime, DateTime>>)await doctorService.GetBusySchedule(doctorId);
            // Loop variables
            DateTime now = UtilityService.RemoveSeconds(DateTime.Now);
            DateTime new_now = now;
            DateTime limit = UtilityService.RemoveSeconds(DateTime.Now.AddHours(2));
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
                    if (UpdateIndex(freeSchedule, old_free) != -1) index = UpdateIndex(freeSchedule, old_free);
                if (UpdateIndex(busySchedule, old_busy) != -1) busyIndex = UpdateIndex(busySchedule, old_busy);
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
                sequence.Add(new RescheduleDTO { PatientId = patientId, DoctorId = doctorId, StartTime = second, EndTime = first, UrgentStartTime = rescheduleTime });
                now = first;
                // Update
                if (flagFree)
                    if (UpdateIndex(freeSchedule, index) != -1) index = UpdateIndex(freeSchedule, index);
                if (UpdateIndex(busySchedule, busyIndex) != -1) busyIndex = UpdateIndex(busySchedule, busyIndex);
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
            for (var i = GetFirstIndex(busySchedule, true); i < busySchedule.Count - 1; i++)
            {
                first = busySchedule[i].Value;
                second = busySchedule[i + 1].Key;
                size = 0;
                DateTime rescheduleTime = now;
                while (size < duration)
                {
                    size += (second - now).Minutes;
                    tempList.Add(new RescheduleDTO { PatientId = patientId, DoctorId = doctorId, StartTime = second, EndTime = first, UrgentStartTime = rescheduleTime });
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
            if (pair.Key > UtilityService.RemoveSeconds(DateTime.Now).AddHours(2)) return -1;
            return lastIndex + 1;
        }

        public int GetFirstIndex(List<KeyValuePair<DateTime, DateTime>> schedule, bool isBusy)
        {
            DateTime now = UtilityService.RemoveSeconds(DateTime.Now);
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

        public async Task<ExaminationDomainModel> AppointUrgent(List<RescheduleDTO> dto, INotificationService notificationService, IRoomService roomService)
        {
            foreach (RescheduleDTO item in dto)
                _ = await RescheduleOne(item, notificationService);
            // Any dto will do
            return await MakeUrgent(dto[0], roomService);
        }

        public async Task<ExaminationDomainModel> RescheduleOne(RescheduleDTO dto, INotificationService notificationService)
        {
            Examination examination = await _examinationRepository.GetByParams(dto.DoctorId, dto.PatientId, dto.StartTime);
            examination.StartTime = dto.RescheduleTime;
            _ = _examinationRepository.Update(examination);
            _examinationRepository.Save();
            _ = await SendNotifications(notificationService, dto.DoctorId, dto.PatientId);
            return ExaminationService.ParseToModel(examination);
        }

        public async Task<Boolean> SendNotifications(INotificationService notificationService, decimal doctorId = 0, decimal patientId = 0)
        {
            KeyValuePair<string, string> content = new KeyValuePair<string, string>("Rescheduling",
                "Your appointment has been rescheduled. Please check your schedule");
            if (doctorId != 0)
                _ = await notificationService.Send(new SendNotificationDTO { IsPatient = false, Content = content, PersonId = doctorId });
            if (patientId != 0)
                _ = await notificationService.Send(new SendNotificationDTO { IsPatient = true, Content = content, PersonId = patientId });
            return true;
        }

        public async Task<ExaminationDomainModel> MakeUrgent(RescheduleDTO dto, IRoomService roomService)
        {
            ExaminationDomainModel examinationModel = new ExaminationDomainModel
            {
                DoctorId = dto.DoctorId,
                IsDeleted = false,
                IsEmergency = true,
                StartTime = dto.UrgentStartTime,
                PatientId = dto.PatientId,
                RoomId = await roomService.GetAvailableRoomId(dto.UrgentStartTime, "examination")
            };
            _ = _examinationRepository.Post(ExaminationService.ParseFromModel(examinationModel));
            _examinationRepository.Save();
            return examinationModel;
        }
    }
}

