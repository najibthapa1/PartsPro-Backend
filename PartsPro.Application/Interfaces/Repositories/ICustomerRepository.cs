using PartsPro.Domain.Entities;

namespace PartsPro.Application.Interfaces.Repositories;

public interface ICustomerRepository : IRepositoryBase<Customer>
{
    Task<Customer?> GetByUserIdAsync(string userId);
}
