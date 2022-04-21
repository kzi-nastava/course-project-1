using System.Data;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using HealthCare.Repositories;

namespace HealthCare.Domain.Services;

public class AnamnesisService : IAnamnesisService
{
    private IAnamnesisRepository _anamnesisRepository;

    public AnamnesisService(IAnamnesisRepository anamnesisRepository) {
        _anamnesisRepository = anamnesisRepository;
    }

    // Async awaits info from database
    // GetAll is the equivalent of SELECT *
    public async Task<IEnumerable<AnamnesisDomainModel>> GetAll()
    {
        var data = await _anamnesisRepository.GetAll();
        if (data == null)
            return null;

        List<AnamnesisDomainModel> results = new List<AnamnesisDomainModel>();
        AnamnesisDomainModel anamnesisModel;
        foreach (var item in data)
        {
            anamnesisModel = new AnamnesisDomainModel
            {
                Description = item.Description,
                doctorId = item.doctorId,
                Examination = item.Examination,
                isDeleted = item.isDeleted,
                patientId = item.patientId,
                roomId = item.roomId,
                StartTime = item.StartTime
            };
            results.Add(anamnesisModel);
        }

        return results;
    }
}