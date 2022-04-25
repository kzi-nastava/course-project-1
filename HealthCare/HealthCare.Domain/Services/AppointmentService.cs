using HealthCare.Data.Entities;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models.ModelsForCreate;
using HealthCare.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthCare.Domain.Services
{
    public class AppointmentService : IAppointmentService
    {
        private IRoomRepository _roomRepository;
        private IExaminationRepository _examinationRepository;

        public AppointmentService(IRoomRepository roomRepository, IExaminationRepository examinationRepository) {
            _roomRepository = roomRepository;
            _examinationRepository = examinationRepository;
        }

        private async Task<decimal> GetAvailableRoomId(CreateExaminationDomainModel examinationModel)
        {
            var rooms = await _roomRepository.GetAllAppointmentRooms("examination");
            foreach (Room room in rooms)
            {
                bool isRoomAvailable = true;
                var examinations = await _examinationRepository.GetAllByRoomId(room.Id);
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

    }
}
