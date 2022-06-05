using HealthCare.Domain.DTOs;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCare.Domain.Services
{
    public class RecommendExaminationService : IRecommendExaminationService
    {
 
        private async Task<List<KeyValuePair<DateTime, DateTime>>> getScehdule(ParamsForRecommendingFreeExaminationsDTO paramsDTO, IDoctorService doctorService)
        {
            IEnumerable<KeyValuePair<DateTime, DateTime>> freeTimes = await doctorService.GetAvailableSchedule(paramsDTO.DoctorId);
            List<KeyValuePair<DateTime, DateTime>> possibleSlots = new List<KeyValuePair<DateTime, DateTime>>();
            foreach (KeyValuePair<DateTime, DateTime> time in freeTimes)
            {
                if (DateTime.Now < time.Value && time.Key < paramsDTO.LastDate)
                {
                    if (DateTime.Now.TimeOfDay < paramsDTO.TimeFrom.TimeOfDay)
                    {
                        DateTime start = new DateTime(time.Key.Year, time.Key.Month, time.Key.Day, paramsDTO.TimeFrom.Hour, paramsDTO.TimeFrom.Minute, 0);
                        possibleSlots.Add(new KeyValuePair<DateTime, DateTime>(start, time.Value));
                    }
                    else
                        possibleSlots.Add(time);
                }
            }
            return possibleSlots;
        }


        private async Task<CUExaminationDTO> checkAviabilityForExamination(ParamsForRecommendingFreeExaminationsDTO paramsDTO, DateTime startTime, IAvailabilityService availabilityService, IPatientService patientService)
        {

            CUExaminationDTO dto = new CUExaminationDTO
            {
                DoctorId = paramsDTO.DoctorId,
                PatientId = paramsDTO.PatientId,
                StartTime = startTime
            };

            try
            {
                await availabilityService.ValidateUserInput(dto, patientService);
            }
            catch (Exception ex)
            {
                return null;
            }
            return dto;
        }
        private async Task<List<CUExaminationDTO>> getRecommendedExaminationsForOneDoctor(ParamsForRecommendingFreeExaminationsDTO paramsDTO, IDoctorService doctorService, IAvailabilityService availabilityService, IPatientService patientService)
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
                if(paramsDTO.TimeFrom.TimeOfDay > startTime.TimeOfDay)
                {
                    startTime = new DateTime(startTime.Year, startTime.Month, startTime.Day, paramsDTO.TimeFrom.Hour, paramsDTO.TimeFrom.Minute, 0);
                }
                //start time is in available range for doctor and patient
                if (startTime.TimeOfDay < paramsDTO.TimeTo.TimeOfDay && startTime < possibleSlots[possibleSlotIndex].Value)
                {
                    CUExaminationDTO recommendedExamination = await checkAviabilityForExamination(paramsDTO, startTime, availabilityService, patientService);
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

        public async Task<IEnumerable<CUExaminationDTO>> RecommendedByDatePriority(ParamsForRecommendingFreeExaminationsDTO paramsDTO, IDoctorService doctorService, decimal numOfExaminations, IAvailabilityService availabilityService, IPatientService patientService)
        {
            List<CUExaminationDTO> recommendedExaminations = await getRecommendedExaminationsForOneDoctor(paramsDTO, doctorService, availabilityService, patientService);
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
                List<CUExaminationDTO> newDoctorExaminations = await getRecommendedExaminationsForOneDoctor(paramsDTO, doctorService, availabilityService, patientService);
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

        public async Task<IEnumerable<CUExaminationDTO>> RecommendedByDoctorPriority(ParamsForRecommendingFreeExaminationsDTO paramsDTO, decimal numOfExaminations, DateTime startTime, IAvailabilityService availabilityService, IPatientService patientService)
        {
            List<CUExaminationDTO> recommendedExaminations = new List<CUExaminationDTO>();
            while (numOfExaminations != 3)
            {
                CUExaminationDTO recommendedExamination = await checkAviabilityForExamination(paramsDTO, startTime, availabilityService, patientService);
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
        public async Task<IEnumerable<CUExaminationDTO>> GetRecommendedExaminations(ParamsForRecommendingFreeExaminationsDTO paramsDTO, IDoctorService doctorService, IAvailabilityService availabilityService, IPatientService patientService)
        {
            List<CUExaminationDTO> recommendedExaminations = await getRecommendedExaminationsForOneDoctor(paramsDTO, doctorService, availabilityService, patientService);
            int numOfExaminations = recommendedExaminations.Count;
            if (numOfExaminations != 3)
            {
                if (paramsDTO.IsDoctorPriority)
                {
                    if (numOfExaminations == 0)
                        return null;
                    // Doctor priority
                    DateTime startTime = recommendedExaminations[numOfExaminations - 1].StartTime.AddMinutes(15);
                    foreach (var examination in await RecommendedByDoctorPriority(paramsDTO, numOfExaminations, startTime, availabilityService, patientService))
                        recommendedExaminations.Add(examination);
                }
                else
                {
                    // Date priority
                    foreach (var examination in await RecommendedByDatePriority(paramsDTO, doctorService, numOfExaminations, availabilityService, patientService))
                        recommendedExaminations.Add(examination);
                }
            }

            return recommendedExaminations;
        }
    }
}
