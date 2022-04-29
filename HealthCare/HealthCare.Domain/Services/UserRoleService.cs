using HealthCare.Data.Entities;
using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using HealthCare.Repositories;

namespace HealthCare.Domain.Services;

public class UserRoleService : IUserRoleService{
    private IUserRoleRepository _userRoleRepository;

    public UserRoleService(IUserRoleRepository userRoleRepository) {
        _userRoleRepository = userRoleRepository;
    }

    // Async awaits info from database
    // GetAll is the equivalent of SELECT *
    public async Task<IEnumerable<UserRoleDomainModel>> GetAll()
    {
        IEnumerable<UserRole> data = await _userRoleRepository.GetAll();
        if (data == null)
            return null;

        List<UserRoleDomainModel> results = new List<UserRoleDomainModel>();
        UserRoleDomainModel userRoleModel;
        foreach (UserRole item in data)
        {
            userRoleModel = new UserRoleDomainModel
            {
                IsDeleted = item.IsDeleted,
                Id = item.Id,
                RoleName = item.RoleName
            };
            results.Add(userRoleModel);
        }

        return results;
    }
    public async Task<IEnumerable<UserRoleDomainModel>> ReadAll()
    {
        IEnumerable<UserRoleDomainModel> userRoles = await GetAll();
        List<UserRoleDomainModel> result = new List<UserRoleDomainModel>();
        foreach (UserRoleDomainModel item in userRoles)
        {
            if (!item.IsDeleted) result.Add(item);
        }
        return result;
    }
}