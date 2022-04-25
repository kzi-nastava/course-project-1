using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthCare.Data.Context;
using HealthCare.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthCare.Repositories {
    public interface IRoomRepository : IRepository<Room> {
        public Task<IEnumerable<Room>> GetAllAppointmentRooms(string roomPurpose);
    }

    public class RoomRepository : IRoomRepository {
        private readonly HealthCareContext _healthCareContext;

        public RoomRepository(HealthCareContext healthCareContext) {
            _healthCareContext = healthCareContext;
        }
        public async Task<IEnumerable<Room>> GetAll() {
            return await _healthCareContext.Rooms
                .Include(x => x.RoomType)
                .Include(x => x.Inventories).ThenInclude(x => x.Equipment).ThenInclude(x => x.EquipmentType)
                .Include(x => x.Operations)
                .ToListAsync();
        }

        // Argument roomPurpose differantiates if the fetched rooms should be rooms for operation/examination
        public async Task<IEnumerable<Room>> GetAllAppointmentRooms(string roomPurpose) {
            return await _healthCareContext.Rooms
                .Include(x => x.RoomType)
                .Where(x => x.RoomType.Purpose == roomPurpose)
                .ToListAsync();
        }

        public void Save()
        {
            _healthCareContext.SaveChanges();
        }
    }
}
