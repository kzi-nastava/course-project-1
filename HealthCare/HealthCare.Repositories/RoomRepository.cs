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

    }
    public class RoomRepository : IRoomRepository {
        private readonly HealthCareContext _healthCareContext;

        public RoomRepository(HealthCareContext healthCareContext) {
            _healthCareContext = healthCareContext;
        }
        public async Task<IEnumerable<Room>> GetAll() {
            return await _healthCareContext.Rooms.ToListAsync();
        }

        public void Save()
        {
            _healthCareContext.SaveChanges();
        }
    }
}
