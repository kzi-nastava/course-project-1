using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using HealthCare.Repositories;

namespace HealthCare.Domain.Services;

public class DoctorService : IDoctorService{
    private IDoctorRepository _doctorRepository;

    public DoctorService(IDoctorRepository doctorRepository) {
        _doctorRepository = doctorRepository;
    }

    // Async awaits info from database
    // GetAll is the equivalent of SELECT *
    public async Task<IEnumerable<DoctorDomainModel>> GetAll()
    {
        var data = await _doctorRepository.GetAll();
        if (data == null)
            return null;

        List<DoctorDomainModel> results = new List<DoctorDomainModel>();
        DoctorDomainModel doctorModel;
        foreach (var item in data)
        {
            doctorModel = new DoctorDomainModel
            {
                isDeleted = item.isDeleted,
                BirthDate = item.BirthDate,
                //Credentials = item.Credentials,
                Email = item.Email,
                Examinations = item.Examinations,
                Id = item.Id,
                Name = item.Name,
                Operations = item.Operations,
                Phone = item.Phone,
                Surname = item.Surname
            };
            results.Add(doctorModel);
        }

        return results;
    }
}