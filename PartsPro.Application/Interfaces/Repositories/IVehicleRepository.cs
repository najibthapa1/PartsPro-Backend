using PartsPro.Domain.Entities;

namespace PartsPro.Application.Interfaces.Repositories;

public interface IVehicleRepository : IRepositoryBase<Vehicle>
{
    Task<Vehicle?> GetByPlateNumberAsync(string plateNumber);
    Task<List<Vehicle>> GetByCustomerIdAsync(int customerId);
}

