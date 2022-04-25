using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthCare.Data.Context;
using HealthCare.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthCare.Repositories {
    public interface IOperationRepository : IRepository<Operation> {
        public Task<IEnumerable<Operation>> GetAllByDoctorId(decimal id);
        public Task<IEnumerable<Operation>> GetAllByPatientId(decimal id);
        public Task<IEnumerable<Operation>> GetAllByRoomId(decimal id);
        public Operation Post(Operation operation);
        public Operation Update(Operation operation);
        public Task<Operation> GetOperation(decimal patientId, decimal doctorId, decimal roomId, DateTime startTime);
    }
    public class OperationRepository : IOperationRepository {
        private readonly HealthCareContext _healthCareContext;

        public OperationRepository(HealthCareContext healthCareContext) {
            _healthCareContext = healthCareContext;
        }
        public async Task<IEnumerable<Operation>> GetAll() {
            return await _healthCareContext.Operations.ToListAsync();
        }

        public async Task<IEnumerable<Operation>> GetAllByDoctorId(decimal id) {
            return await _healthCareContext.Operations
                .Where(x => x.DoctorId == id)
                .Where(x => x.isDeleted == false)
                .ToListAsync();
        }

        public async Task<IEnumerable<Operation>> GetAllByPatientId(decimal id)
        {
            return await _healthCareContext.Operations
                .Where(x => x.PatientId == id)
                .Where(x => x.isDeleted == false)
                .ToListAsync();
        }

        public async Task<IEnumerable<Operation>> GetAllByRoomId(decimal id)
        {
            return await _healthCareContext.Operations
                .Where(x => x.RoomId == id)
                .Where(x => x.isDeleted == false)
                .ToListAsync();
        }

        public async Task<Operation> GetOperation(decimal patientId, decimal doctorId, decimal roomId, DateTime startTime)
        {
            return await _healthCareContext.Operations
                .Where(x => x.RoomId == roomId)
                .Where(x => x.PatientId == patientId)
                .Where(x => x.DoctorId == doctorId)
                .Where(x => x.StartTime == startTime)
                .FirstOrDefaultAsync();
        }

        public Operation Post(Operation operation)
        {
            var result = _healthCareContext.Operations.Add(operation);
            return result.Entity;
        }

        public Operation Update(Operation operation)
        {
            var updatedEntry = _healthCareContext.Operations.Attach(operation);
            _healthCareContext.Entry(operation).State = EntityState.Modified;
            return updatedEntry.Entity;
        }

        public void Save()
        {
            _healthCareContext.SaveChanges();
        }
    }
}
