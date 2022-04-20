using HealthCare.Domain.Interfaces;
using HealthCare.Domain.Models;
using HealthCare.Repositories;

namespace HealthCare.Domain.Services;

public class UserRoleService : IUserRoleService{
    private IUserRoleRepository _userRoleRepository;

    public UserRoleService(IUserRoleRepository userRoleRepository) {
        _userRoleRepository = userRoleRepository;
    }

    public Task<IEnumerable<UserRoleDomainModel>> GetAll()
    {
        throw new NotImplementedException();
    }
}