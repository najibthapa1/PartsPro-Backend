using PartsPro.Domain.Entities;

namespace PartsPro.Application.Interfaces.Repositories;

public interface ISaleRepository : IRepositoryBase<Sale>
{
    Task<Sale?> GetByIdWithItemsAsync(int id);

    Task<List<Sale>> GetAllWithItemsAsync();
    Task<List<Sale>> GetByCustomerIdAsync(int customerId);
    Task<Sale?> GetByIdWithItemsAndCustomerAsync(int id);
    void Update(Sale sale);
}