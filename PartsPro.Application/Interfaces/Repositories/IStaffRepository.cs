using PartsPro.Domain.Entities;

namespace PartsPro.Application.Interfaces.Repositories;

public interface IStaffRepository : IRepositoryBase<Staff>
{
    Task<Staff?> GetByUserIdAsync(string userId);
}
