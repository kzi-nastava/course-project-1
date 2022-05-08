using HealthCare.Domain.Models;

namespace HealthCare.Domain.Interfaces;

public interface IDoctorService : IService<DoctorDomainModel>
{
    public Task<IEnumerable<DoctorDomainModel>> ReadAll();

    public Task<IEnumerable<KeyValuePair<DateTime, DateTime>>> GetAvailableSchedule(decimal doctorId, decimal duration = 15);
}