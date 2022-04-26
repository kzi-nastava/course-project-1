using System.Data;
using HealthCare.Data.Entities;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using HealthCare.Domain.Models.ModelsForCreate;
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
                isDeleted = item.isDeleted,
                patientId = item.patientId,
                roomId = item.roomId,
                StartTime = item.StartTime
            };
            results.Add(anamnesisModel);
        }

        return results;
    }

    public async Task<AnamnesisDomainModel> Create(CreateAnamnesisDomainModel createModel)
    {
        Anamnesis anamesis = new Anamnesis
        {
            Description = createModel.Description,
            patientId = createModel.patientId,
            doctorId = createModel.doctorId,
            roomId = createModel.roomId,
            StartTime = createModel.StartTime,
            isDeleted = false
        };
        _ = _anamnesisRepository.Post(anamesis);
        _anamnesisRepository.Save();

        return parseToModel(anamesis);
    }

    public AnamnesisDomainModel parseToModel(Anamnesis anamnesis)
    {
        AnamnesisDomainModel model = new AnamnesisDomainModel
        {
            Description = anamnesis.Description,
            patientId = anamnesis.patientId,
            doctorId = anamnesis.doctorId,
            roomId = anamnesis.roomId,
            StartTime = anamnesis.StartTime,
            isDeleted = anamnesis.isDeleted
        };

        return model;
    }
}